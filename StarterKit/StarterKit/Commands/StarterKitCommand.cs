using StarterKit.Core;
using StarterKit.Localization;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vintagestory.API.Common;
using Vintagestory.API.Server;

namespace StarterKit.Commands
{
    /// <summary>
    /// Команда стартового набора.
    /// </summary>
    public sealed class StarterKitCommand : Command
    {
        /// <summary>
        /// Количество полученных игроком стартовых наборов.
        /// </summary>
        public const string ReceivesKey = "StarterKit:Receives";

        /// <summary>
        /// Конструктор команды стартового набора.
        /// </summary>
        /// <param name="api">API сервера.</param>
        /// <param name="config">Конфигурация мода.</param>
        internal StarterKitCommand(ICoreServerAPI api, StarterKitConfig config) : base(api, config)
        {
            CommandArgumentParsers parsers = API.ChatCommands.Parsers;
            IChatCommand sk = API.ChatCommands.GetOrCreate("starterkit");
            sk.RequiresPrivilege(Privilege.chat);
            sk.RequiresPlayer();
            sk.WithRootAlias("sk");
            sk.WithDescription("Command to get starter kit.");
            sk.HandleWith(GetStarterKitHandle);

            IChatCommand update = sk.BeginSubCommand("update");
            update.RequiresPrivilege(Privilege.root);
            update.WithDescription($"Updates the {StarterKitModSystem.ModName} configuration");
            update.HandleWith(ConfigurationUpdateHandle);
            update.EndSubCommand();

            IChatCommand resetAll = sk.BeginSubCommand("resetall");
            resetAll.RequiresPrivilege(Privilege.root);
            resetAll.WithDescription("Resets the starter pack receipt counter for all players.");
            resetAll.HandleWith(ResetAllHandle);
            resetAll.EndSubCommand();

            IChatCommand reset = sk.BeginSubCommand("reset");
            reset.RequiresPrivilege(Privilege.root);
            reset.WithDescription("Resets the counter for starting pack receipts for players specified by commas.");
            reset.WithArgs(parsers.All("Players"));
            reset.HandleWith(ResetHandle);
            reset.EndSubCommand();
        }

        /// <summary>
        /// Обработчик команды получения стартового набора.
        /// </summary>
        /// <param name="args">Аргументы команды.</param>
        /// <returns>Результат выполнения.</returns>
        public TextCommandResult GetStarterKitHandle(TextCommandCallingArgs args)
        {
            // Определение игрока и его информации
            IPlayer? player = args.Caller.Player;
            IServerPlayer? serverPlayer = API.Server.Players.FirstOrDefault(x => x.PlayerUID == player?.PlayerUID);
            Dictionary<string, string>? data = serverPlayer?.ServerData?.CustomPlayerData;
            if (player == null || serverPlayer == null || data == null)
                return TextCommandResult.Error(Localizer.Get("Players.PlayerOrDataNotFound"));

            // Определение количества полученный наборов
            int receives;
            if (data.ContainsKey(ReceivesKey))
            {
                // Исправление записи
                if (!int.TryParse(data[ReceivesKey], out receives))
                {
                    data[ReceivesKey] = "0";
                    receives = 0;
                }
            }
            else
            {
                // Регистрация записи
                data.Add(ReceivesKey, "0");
                receives = 0;
            }

            // Больше наборов получить нельзя
            if (receives >= Config.PermittedReceives)
                return TextCommandResult.Success(Localizer.Get(serverPlayer.LanguageCode, "Players.AllKitsReceived"));

            // Формирование набора
            List<ItemStack> stacks = Config.CreateItems();
            if (stacks.Count == 0)
                return TextCommandResult.Success(Localizer.Get(serverPlayer.LanguageCode, "Creator.EmptyOrErrorKit"));

            // Выдача набора
            bool hasSlots = true;
            for (int i = 0; i < stacks.Count;)
            {
                // Выдача под ноги, если инвентарь занят
                ItemStack stack = stacks[i];
                if (!hasSlots)
                {
                    ++i;
                    API.World.SpawnItemEntity(stack, args.Caller.Pos);
                    continue;
                }

                // Выдача в инвентарь
                hasSlots = player.InventoryManager.TryGiveItemstack(stack, true);
                if (hasSlots)
                    ++i;
            }

            // Обновление информации игрока
            data[ReceivesKey] = (receives + 1).ToString();
            return TextCommandResult.Success(Localizer.Get(serverPlayer.LanguageCode, "Players.KitReceived"));
        }
        /// <summary>
        /// Обработчик команды обновления конфигурации.
        /// </summary>
        /// <param name="args">Аргументы команды.</param>
        /// <returns>Результат выполнения.</returns>
        public TextCommandResult ConfigurationUpdateHandle(TextCommandCallingArgs args)
        {
            // Сохранение старой ссылки и обновление конфигурации
            StackInfo?[] items = Config.Items;
            Config.Load();

            // Оповещение о результате
            if (object.ReferenceEquals(items, Config.Items))
                return TextCommandResult.Error(Localizer.Get("IO.ConfigReadException"));
            else
                return TextCommandResult.Success(Localizer.Get("Updating.Updated"));
        }
        /// <summary>
        /// Обработчик команды сброса количества полученных игроками стартовых наборов.
        /// </summary>
        /// <param name="args">Аргументы команды.</param>
        /// <returns>Результат выполнения.</returns>
        public TextCommandResult ResetAllHandle(TextCommandCallingArgs args)
        {
            foreach (IServerPlayer player in API.Server.Players)
                player.ServerData.CustomPlayerData.Remove(ReceivesKey);

            return TextCommandResult.Success(Localizer.Get("Reset.All"));
        }
        /// <summary>
        /// Обработчик команды сброса количества полученных игроками стартовых наборов.
        /// </summary>
        /// <param name="args">Аргументы команды.</param>
        /// <returns>Результат выполнения.</returns>
        public TextCommandResult ResetHandle(TextCommandCallingArgs args)
        {
            // Проверка списка никнеймов
            string? nicknamesArg = args[0] as string;
            if (nicknamesArg == null)
                return TextCommandResult.Error(Localizer.Get("Reset.Specified.NoPlayers"));

            // Проход по никнеймам
            List<string> notFound = new List<string>();
            List<string> nicknames = nicknamesArg.Split(',', System.StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToList();
            foreach (string nickname in nicknames)
            {
                // Поиск игрока
                IServerPlayer? player = API.Server.Players.FirstOrDefault(x => x.PlayerName == nickname);
                if (player == null)
                {
                    notFound.Add(nickname);
                    continue;
                }

                // Удаление информации
                player.ServerData.CustomPlayerData.Remove(ReceivesKey);
            }

            // Оповещение о результате
            if (notFound.Count == 0)
                return TextCommandResult.Success(Localizer.Get("Reset.Specified.Full"));
            else
                return TextCommandResult.Success(Localizer.Get("Reset.Specified.Partial", string.Join(", ", notFound)));
        }
    }
}