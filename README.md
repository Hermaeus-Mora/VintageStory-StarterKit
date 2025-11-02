## Описание
Данный мод позволяет добавлять на сервер стартовый набор.

## Команды
**/starterkit** или **/sk** – выдаёт игроку стартовый набор (требует привилегию **chat**; должна быть выполнена игроком);\
**/starterkit update** – обновляет на работающем сервере кофигурацию из файла /VintagestoryData/ModConfig/StarterKit.json (требует привилегию **root**);\
**/starterkit resetall** – сбрасывает для всех игроков количество уже полученных ими стартовых наборов (требует привилегию **root**). Данная команда в настоящий момент также удаляет информацию, вносимую модом перед удалением с сервера;\
**/starterkir reset player1, player2, ...** – делает то же самое, что resetall, но для указанных игроков (требует привилегию **root**).

## Конфигурация
**PermittedReceives** – (System.Int32) количество стартовых наборов, которое может получить каждый игрок. Значение ≤0 отключает возможность получения стартовых наборов;\
**Items** – (StackInfo[]) информация о стаке предметов, входящих в стартовый набор.

Предметы в Items добавляются согласно правил JSON-документов. Конфигурация изменяется только через файл, но **/starterkit update** позволяет не перезапускать сервер.

Пример конфигурации (каждый игрок может получить 2 стартовых набора, состоящих из одной темпоральной шестерни и 16 хвощевых припарок):\
<details>
  <summary>Пример</summary>

  <pre>{
  "PermittedReceives": 2,
  "Items": [
    {
      "Code": "game:gear-temporal",
      "Amount": 1
    },
    {
      "Code": "game:poultice-linen-horsetail",
      "Amount": 16
    }
  ]
}</pre>
</details>


## Модель StackInfo
**Code** – (System.String) код предмета, например "game:gear-temporal";\
**Amount** – (System.Int32) количество предметов в выдаваемом стаке. Значение ≤0 будет игнорироваться;\
**Attributes** – (StackAttribute[]) атрибуты стака предметов.

## Модель StackAttribute
**Type** – (System.String) тип значения атрибута;\
**Key** – (System.String) ключ атрибута;\
**Value** – (System.String) строковое значение атрибута.

Я не проверял все возмножные отклонения значений атрибутов от нормы, но будьте с ними осторожны.

<details>
  <summary>Допустимые типы значений атрибутов</summary>

  "bool" – System.Boolean;\
  "int" – System.Int32;\
  "long" – System.Int64;\
  "float" – System.Single;\
  "double" – System.Double;\
  "byte[]" – System.Byte[], в файле конфигурации записывется как hex-строка;\
  "string" – System.String.
</details>

<details>
  <summary>Прмеры</summary>

  <details>
    <summary>Поломанная стальная кирка</summary>
    <pre>{
  "Code": "game:pickaxe-steel",
  "Amount": 2,
  "Attributes": [
    { "Type": "int", "Key": "durability", "Value": "347" }
  ]
}</pre>
  </details>

  <details>
    <summary>Подписанная книга</summary>
    <pre>{
  "Code": "game:book-normal-brickred",
  "Amount": 1,
  "Attributes": [
    { "Type": "string", "Key": "title", "Value": "Заголовок книги" },
    { "Type": "string", "Key": "text", "Value": "Текст книги\nСтрока 2" },
    { "Type": "string", "Key": "signedby", "Value": "Tulikettu" },
    { "Type": "string", "Key": "signedbyuid", "Value": "gC8QxMPxcegHcHCd7dqwhzhf" }
  ]
}</pre>
  </details>
</details>
