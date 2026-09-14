# Fill Jars From Water

Мод для **7 Days to Die V3.2**: правый клик пустой банкой по реке или озеру набирает **мутную воду** (`drinkJarRiverWater`) **в инвентарь**. Потом её нужно вскипятить в котелке.

Vanilla V3 использует `CollectWater` и новую жидкость. На PGz / кастомной воде набор часто не работает, а если и работает — банка падает на землю. Этот мод оставляет `CollectWater`, добавляет Harmony-заполнение через `World.IsWater` и **глушит ванильный экшен после удачного набора**, чтобы банка не выпадала рядом.

Сделано в [Grok Build](https://grok.com).

---

## Требования

- 7 Days to Die **V3.2** (Steam)
- **Easy Anti-Cheat выключен** (в моде есть DLL)
- Папка игры `Mods\0_TFP_Harmony` на месте — **не удалять**
- Пустая банка `drinkJarEmpty` в хотбаре

## Установка (Windows)

1. Полностью закройте игру (не только в меню — процесс `7DaysToDie.exe` не должен висеть).
2. Скачайте `FillJarsFromWater-1.0.2.zip` со страницы [Releases](https://github.com/daaag0n00969/7dtd-fill-jars-from-water/releases/latest).
3. Распакуйте архив так, чтобы получилось:

```
%APPDATA%\7DaysToDie\Mods\FillJarsFromWater\ModInfo.xml
%APPDATA%\7DaysToDie\Mods\FillJarsFromWater\FillJarsFromWater.dll
%APPDATA%\7DaysToDie\Mods\FillJarsFromWater\Config\items.xml
```

Путь `%APPDATA%\7DaysToDie\Mods` обычно равен:

```
C:\Users\<ИМЯ>\AppData\Roaming\7DaysToDie\Mods
```

4. Запустите игру **без EAC** (ярлык Steam с `-noeac` или лаунчер с UseEAC = false).
5. Положите **пустую банку** в руку / хотбар, смотрите на поверхность воды, **зажмите ПКМ**.

Одна банка в слоте заменяется мутной водой на месте. Стопка банок: одна пустая тратится, мутная уходит в рюкзак (или на другой слот хотбара). На землю падает **только если инвентарь и хотбар полностью забиты**.

Кипятите мутную воду в котелке, как в ванили. ЛКМ не используйте — набор на ПКМ.

## Как понять, что мод загрузился

В логе `7DaysToDie_log.txt` (в `%APPDATA%\7DaysToDie\`) должна быть строка:

```
[FillJarsFromWater] loaded 1.0.2
```

Если её нет — EAC включён, DLL не подхватилась, или папка мода лежит не в `Mods\FillJarsFromWater`.

## Сборка из исходников

Нужны .NET SDK и установленная игра.

```
dotnet build -c Release
```

Путь к игре задаётся свойством `GamePath` в `FillJarsFromWater.csproj`.

## Удаление

Удалите папку `Mods\FillJarsFromWater`.

## Лицензия

MIT

---

# English

Right-click an **empty jar** while looking at a **river or lake**. It becomes **murky water** in your inventory (same hotbar slot if you were holding a single jar). Boil it in a cooking pot.

Vanilla V3 `CollectWater` often fails on PGz / custom RWG water, and a successful fill can still `ItemDropServer` the jar on the ground. This mod keeps CollectWater, adds a Harmony fill via `World.IsWater`, and **suppresses vanilla CollectWater after a successful fill** so the jar stays in the inventory.

## Requirements

- 7 Days to Die **V3.2**
- **EAC off**
- Game `Mods\0_TFP_Harmony` present

## Install

1. Close the game.
2. Download `FillJarsFromWater-1.0.2.zip` from [Releases](https://github.com/daaag0n00969/7dtd-fill-jars-from-water/releases/latest).
3. Extract to `%APPDATA%\7DaysToDie\Mods\FillJarsFromWater\`
4. Launch without EAC.
5. Put `drinkJarEmpty` on the hotbar, look at the water surface, **hold RMB**.

Do not use LMB. Murky water (`drinkJarRiverWater`) should be boiled.

## Changelog

- **1.0.2** — Murky water stays in inventory; vanilla CollectWater is blocked after a fill so it cannot drop a second jar.
- **1.0.1** — Replace held empty jar in-place; bag/hotbar fallback.
- **1.0.0** — First Harmony fill.

## License

MIT
