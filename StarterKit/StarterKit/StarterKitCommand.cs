using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vintagestory.API.Common;
using Vintagestory.API.Server;

namespace StarterKit
{
    /// <summary>
    /// Команда стартового набора.
    /// </summary>
    public sealed class StarterKitCommand
    {
        /// <summary>
        /// Количество полученных игроком стартовых наборов.
        /// </summary>
        public const string ReceivesKey = "StarterKit:Receives";

        /// <summary>
        /// API сервера.
        /// </summary>
        private readonly ICoreServerAPI API;
        /// <summary>
        /// Конфигурация мода.
        /// </summary>
        private readonly StarterKitConfig Config;

        /// <summary>
        /// Конструктор команды стартового набора.
        /// </summary>
        /// <param name="api">API сервера.</param>
        /// <param name="config">Конфигурация мода.</param>
        public StarterKitCommand(ICoreServerAPI api, StarterKitConfig config)
        {
            API = api;
            Config = config;

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
            IPlayer? player = args.Caller.Player;
            Dictionary<string, string>? data = API.Server.Players.FirstOrDefault(x => x.PlayerUID == player?.PlayerUID)?.ServerData?.CustomPlayerData;
            if (player == null || data == null)
                return TextCommandResult.Error("Player not found.");

            int receives;
            if (data.ContainsKey(ReceivesKey))
            {
                if (!int.TryParse(data[ReceivesKey], out receives))
                {
                    data[ReceivesKey] = "0";
                    receives = 0;
                }
            }
            else
            {
                data.Add(ReceivesKey, "0");
                receives = 0;
            }

            if (receives >= Config.PermittedReceives)
                return TextCommandResult.Success("You have already received all the starter kits.");

            List<ItemStack> stacks = Config.CreateItems();
            if (stacks.Count == 0)
                return TextCommandResult.Success("The starter kit is empty or there was an error generating it.");

            bool hasSlots = true;
            for (int i = 0; i < stacks.Count;)
            {
                ItemStack stack = stacks[i];
                if (!hasSlots)
                {
                    ++i;
                    API.World.SpawnItemEntity(stack, args.Caller.Pos);
                    continue;
                }

                hasSlots = player.InventoryManager.TryGiveItemstack(stack, true);
                if (hasSlots)
                    ++i;
            }

            data[ReceivesKey] = (receives + 1).ToString();
            return TextCommandResult.Success("Starter kit received.");
        }
        /// <summary>
        /// Обработчик команды обновления конфигурации.
        /// </summary>
        /// <param name="args">Аргументы команды.</param>
        /// <returns>Результат выполнения.</returns>
        public TextCommandResult ConfigurationUpdateHandle(TextCommandCallingArgs args)
        {
            ItemInfo[] items = Config.Items;
            Config.Load();

            if (object.ReferenceEquals(items, Config.Items))
                return TextCommandResult.Error($"Failed to load {StarterKitModSystem.ModName} configuration.");
            else
                return TextCommandResult.Success("Configuration was updated");
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

            return TextCommandResult.Success("Success");
        }
        /// <summary>
        /// Обработчик команды сброса количества полученных игроками стартовых наборов.
        /// </summary>
        /// <param name="args">Аргументы команды.</param>
        /// <returns>Результат выполнения.</returns>
        public TextCommandResult ResetHandle(TextCommandCallingArgs args)
        {
            string? nicknamesArg = args[0] as string;
            if (nicknamesArg == null)
                return TextCommandResult.Error("No command arguments were found.");

            List<string> notFound = new List<string>();
            List<string> nicknames = nicknamesArg.Split(',', System.StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToList();
            foreach (string nickname in nicknames)
            {
                IServerPlayer? player = API.Server.Players.FirstOrDefault(x => x.PlayerName == nickname);
                if (player == null)
                {
                    notFound.Add(nickname);
                    continue;
                }

                player.ServerData.CustomPlayerData.Remove(ReceivesKey);
            }

            StringBuilder builder = new StringBuilder("Success");
            if (notFound.Count > 0)
                builder.Append($", but players ({string.Join(", ", notFound)}) were not found.");

            return TextCommandResult.Success(builder.ToString());
        }
    }
}