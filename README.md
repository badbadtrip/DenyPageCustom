# DenyPageCustom

[![Release](https://img.shields.io/github/v/release/badbadtrip/DenyPageCustom?label=version&style=flat-square)](https://github.com/badbadtrip/DenyPageCustom/releases)
[![License: MIT](https://img.shields.io/badge/license-MIT-blue?style=flat-square)](LICENSE)
[![Lampac](https://img.shields.io/badge/Lampac-NextGen-blueviolet?style=flat-square)](https://github.com/lampac-nextgen/lampac)
[![Platform](https://img.shields.io/badge/platform-Roslyn%20%7C%20.NET-informational?style=flat-square)](https://github.com/lampac-nextgen/lampac)
[![DeepWiki](https://deepwiki.com/badge.svg)](https://deepwiki.com/badbadtrip/DenyPageCustom)

> Динамический плагин для [Lampac NextGen](https://github.com/lampac-nextgen/lampac) — генерирует кастомную страницу авторизации `plugins/override/deny.js` на основе секции `[DenyPage]` в `init.conf`. Горячая перезагрузка, QR-код Telegram, адаптивная вёрстка.

---

## Содержание

- **[Скриншоты](#скриншоты)**
- **[Возможности](#возможности)**
- **[Предварительные требования](#предварительные-требования)**
- **[Установка](#установка)**
- **[Конфигурация](#конфигурация)**
- **[Поля конфигурации](#поля-конфигурации)**
- **[Логика авторизации](#логика-авторизации)**
- **[Разработка](#разработка)**
  - [Архитектура](#архитектура)
  - [Адаптивная вёрстка](#адаптивная-вёрстка)
  - [Добавление нового поля конфигурации](#добавление-нового-поля-конфигурации)
  - [Структура сгенерированного файла](#структура-сгенерированного-файла)
  - [manifest.json](#manifestjson)
- **[Лицензия](#лицензия)**

---

## Скриншоты

<p align="center">
  <img src="screenshots/lampac.png" width="440" alt="Планшет / Desktop">
  <img src="screenshots/lampacmob.png" width="230" alt="Мобильный">
</p>

---

## Возможности

| | |
|---|---|
| **Кастомизация текстов** | Заголовок, подзаголовок, шаги инструкции, подписи QR-блока — всё через `init.conf` |
| **QR-код Telegram** | Генерируется автоматически для любого бота/канала; скрывается, если `tg_target` не задан |
| **Горячая перезагрузка** | `"dynamic": true` — изменения применяются без перезапуска сервера |
| **Хэш-диффинг** | Файл перезаписывается только при реальных изменениях конфига |
| **Защита от XSS** | Текст из конфига вставляется через `textContent`/`href`, а не в разметку — HTML не интерпретируется |
| **Адаптивный дизайн** | TV, Desktop, планшет, мобильный, ландшафт |

---

## Предварительные требования

> [!IMPORTANT]
> DenyPageCustom — это только генератор фронтенда (`deny.js`). Он не хранит пароли и ничего не проверяет — вся проверка происходит на стороне самого Lampac NextGen. Без пунктов ниже плагин установится и нарисует форму, но экран блокировки работать не будет.

### 1. Включить `accsdb` в основном `init.conf` сервера

`accsdb` — встроенная функция самого Lampac NextGen (не часть этого плагина). Именно она обслуживает эндпоинт `/testaccsdb`, к которому обращается сгенерированный `deny.js` — проверяет пароль, ведёт лимиты и блокировки.

```json
"accsdb": {
  "enable": true,
  "whitepattern": "^/adminpanel",
  "shared_passwd": "ваш_пароль",
  "shared_daytime": 3660,
  "maxip_hour": 10,
  "maxrequest_hour": 2000,
  "maxlock_day": 3,
  "blocked_hour": 36,
  "authMesage": "Текст при успешной авторизации",
  "denyMesage": "Текст при отказе в доступе",
  "accounts": {},
  "users": []
}
```

| Поле | Описание |
|---|---|
| `enable` | без `true` эндпоинт `/testaccsdb` не работает — DenyPage бесполезен |
| `whitepattern` | regex путей, которые пропускаются без проверки пароля (статика, вебхуки бота и т.п.) |
| `shared_passwd` | общий пароль на всех, если не используются персональные `accounts`/`users` |
| `shared_daytime` | сколько секунд действует сессия после успешного ввода пароля |
| `maxip_hour` / `maxrequest_hour` | лимиты запросов с одного IP в час |
| `maxlock_day` / `blocked_hour` | блокировка после N неудачных попыток в день, на M часов |
| `accounts` / `users` | персональные пароли/аккаунты — заполняются вручную или сторонним ботом |

> [!NOTE]
> Это конфигурация ядра Lampac NextGen, а не DenyPageCustom — полный список ключей и их поведение смотрите в документации самого Lampac NextGen.

### 2. (опционально) Telegram-бот для выдачи паролей

QR-код и кнопка «Написать боту» (`tg_target` в `[DenyPage]`) — это просто ссылка. Чтобы пользователь реально получал пароль в Telegram, нужен отдельный бот, который в DenyPageCustom **не входит**:

1. Зарегистрируйте бота через [@BotFather](https://t.me/BotFather) → команда `/newbot` → получите `bot_token`.
2. Настройте у бота собственную логику выдачи пароля (отдельный сервис/скрипт, слушающий сообщения бота) — он должен либо сообщать `shared_passwd`, либо генерировать персональный пароль и добавлять его в `accsdb.accounts`/`accsdb.users`.
3. Укажите username бота в `tg_target` (см. [Форматы tg_target](#форматы-tg_target)) — DenyPageCustom только строит по нему QR-код и ссылку, саму выдачу пароля обеспечивает бот.

Если DenyPage должен работать без Telegram — просто не указывайте `tg_target` (или `show_qr: false`), тогда останется только форма ввода пароля, проверяемая через `accsdb.shared_passwd`.

> [!WARNING]
> `bot_token` — секрет. Не публикуйте и не коммитьте его в репозитории/конфиги. Если токен утёк — немедленно перевыпустите его через BotFather (`/token` → Revoke).

---

## Установка

> [!NOTE]
> Lampac NextGen использует **Roslyn** — `.cs` файлы компилируются сервером автоматически. Предварительная сборка в DLL не требуется.

**1.** Скопируйте папку `DenyPageCustom` в директорию `mods/` рядом с исполняемым файлом Lampac:

```
mods/
└── DenyPageCustom/
    ├── manifest.json
    ├── ModInit.cs
    ├── DenyPageGenerator.cs
    └── Models/
        └── DenyPageConf.cs
```

**2.** Перезапустите сервер — Roslyn скомпилирует модуль автоматически.

**3.** При `"dynamic": true` (уже в `manifest.json`) последующие изменения `.cs` файлов применяются **без перезапуска**.

> [!TIP]
> Файл `plugins/override/deny.js` создаётся автоматически в директории установки сервера — редактировать его вручную не нужно, он перезаписывается при каждом изменении конфига.

---

## Конфигурация

Параметры задаются в секции `[DenyPage]` файла `init.conf`.

> [!TIP]
> Изменения применяются автоматически — перезапуск сервера не требуется.

```json
"DenyPage": {
  "tg_target": "@YourBot",
  "show_qr": true,
  "page_title": "Вход в систему",
  "page_subtitle": "Для доступа к серверу введите пароль, выданный администратором.",
  "step1_text": "Нажмите «Войти» и введите пароль.",
  "qr_caption": "Нет пароля?",
  "qr_subcaption": "Отсканируй QR или нажми кнопку, чтобы написать боту.",
  "tg_button_text": "Написать боту"
}
```

---

## Поля конфигурации

<details>
<summary><b>Все поля с описанием</b></summary>

<br>

| Поле | Тип | Описание |
|---|---|---|
| `tg_target` | `string` | Управляет QR-блоком. Принимает `@username`, `https://t.me/…` или `tg://` |
| `show_qr` | `bool` | QR отображается только если `tg_target` задан **и** `show_qr = true` |
| `page_title` | `string` | Заголовок страницы ✓ |
| `page_subtitle` | `string` | Подзаголовок ✓ |
| `step1_text` | `string` | Текст шага 01 ✓ |
| `qr_caption` | `string` | Заголовок QR-блока ✓ |
| `qr_subcaption` | `string` | Подпись QR-блока ✓ |
| `tg_button_text` | `string` | Текст кнопки Telegram ✓ |
| `page_badge` | `string` | Заголовок-бейдж над формой ✓ |

### Форматы `tg_target`

> [!TIP]
> Все варианты нормализуются автоматически — достаточно указать `@username`.

```
@mybotname          →  https://t.me/mybotname
mybotname           →  https://t.me/mybotname
https://t.me/bot    →  https://t.me/bot         (без изменений)
tg://resolve?...    →  tg://resolve?...          (без изменений)
```

</details>

---

## Логика авторизации

<details>
<summary><b>Как работает deny.js</b></summary>

<br>

Сгенерированный `deny.js` внедряет две функции:

**`checkAutch()`** — вызывается немедленно при загрузке. Отправляет GET на `{localhost}/testaccsdb`. Если `res.accsdb = true`:
- Скрывает UI (`#app`), устанавливает `window.sync_disable = true`
- Через 500 мс вызывает `addDevice()`

**`addDevice(message)`** — рендерит полноэкранную карточку входа в `document.body`. Кнопка «Войти» открывает `Lampa.Input.edit(...)` — встроенную текстовую клавиатуру Lampa (та же, что использует стоковый `deny.js`), а не самодельный HTML `<input>`. Она уже умеет работать на Apple TV/tvOS и других TV-платформах без ручных фокус-хаков.

```
POST {localhost}/testaccsdb?account_email=<пароль>&uid=<uid>
```

| Результат | Поведение |
|---|---|
| `success = true`, `uid` задан | Аккаунт создан — показывает UID, редирект на `/` через 3 сек |
| `success = true`, `uid` не задан | Сохраняет пароль как `lampac_unic_id`, редирект на `/` |
| `success = false` | Показывает «Неправильный пароль» |
| Ошибка сети | Показывает «Ошибка соединения» |

> [!NOTE]
> QR-изображение загружается в runtime с `api.qrserver.com` — не бандлится в плагин. Требуется доступ в интернет на устройстве пользователя.

</details>

---

## Разработка

<details>
<summary><b>Архитектура</b></summary>

<br>

```
DenyPageCustom/
├── manifest.json          # Метаданные плагина для Lampac
├── ModInit.cs             # Точка входа: IModuleLoaded, таймер, подписка на события
├── DenyPageGenerator.cs   # Генератор JS-файла (чистый static builder)
└── Models/
    └── DenyPageConf.cs    # Модель конфигурации с дефолтами
```

**`ModInit.cs`** — точка входа (`IModuleLoaded`):
- При загрузке создаёт директорию вывода, вызывает `SyncAndGenerate()` один раз
- Подписывается на `EventListener.UpdateInitFile` (обновление `init.conf`)
- Таймер на 3 секунды — резервный механизм синхронизации

**`DenyPageGenerator.cs`** — статический построитель:
- `Build(DenyPageConf)` возвращает полное содержимое `deny.js`
- QR-колонка генерируется только если `tg_target` задан и `show_qr = true`

**`Models/DenyPageConf.cs`** — модель конфигурации:
- Заполняется через `ModuleInvoke.Init("DenyPage", new DenyPageConf())` из `init.conf`
- Все поля имеют значения по умолчанию

</details>

<details>
<summary><b>Адаптивная вёрстка</b></summary>

<br>

| Условие | Поведение |
|---|---|
| TV (≥ 1400px) | Макс. ширина 1100px, высота 85vh, QR 180×180px |
| Desktop (> 900px) | Два столбца: форма слева, QR-блок справа (280px) |
| Планшет (700–900px) | Два столбца, QR-блок 240px |
| Мобильный (≤ 700px) | Один столбец: QR-блок над формой, QR 120×120px |
| Ландшафт (h ≤ 500px) | QR-блок справа (border-left), ширина 220px |
| Маленький экран (≤ 420px) | Минимальные отступы (24px/20px) |

</details>

<details>
<summary><b>Добавление нового поля конфигурации</b></summary>

<br>

1. Добавьте свойство в `Models/DenyPageConf.cs` с дефолтным значением
2. Используйте значение в `DenyPageGenerator.Build()` — включите в генерируемый HTML/JS
3. Обновите таблицу полей в этом README

</details>

<details>
<summary><b>Структура сгенерированного файла</b></summary>

<br>

```
deny.js
├── CSS (инлайн, через <style>)
├── SVG-иконки как JS-переменные (warn, tg)
├── function addDevice(message)   — форма входа + обработчики
└── function checkAutch()         — проверка авторизации + вызов addDevice
    └── checkAutch();             — немедленный вызов в конце файла
```

</details>

<details>
<summary><b>manifest.json</b></summary>

<br>

```json
{
  "name": "DenyPageCustom",
  "enable": true,
  "dynamic": true
}
```

| Поле | Описание |
|---|---|
| `name` | Идентификатор плагина (должен совпадать с именем DLL) |
| `enable` | `true` — плагин активируется при старте |
| `dynamic` | `true` — горячая перезагрузка при изменении `.cs` |

</details>

---

## Лицензия

[MIT](LICENSE)
