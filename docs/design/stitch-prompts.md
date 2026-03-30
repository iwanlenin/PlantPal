# PlantPal — UI Design Prompts (Stitch)

These prompts are for generating UI mockups in Stitch (or similar design tools) before building each version's screens in MAUI.

---

## MVP — Confirmed ✅

> Dashboard, Add Plant, Empty state designs confirmed.

```
Design a mobile app called PlantPal for iOS and Android.

PlantPal helps users track watering schedules for their indoor plants. The app is calm and
nature-inspired — it should feel like a tool a plant lover would enjoy opening every day.

Design 3 screens:

SCREEN 1 — Dashboard
The main screen the user sees every day. It shows two groups of plants: those that need water
today (or are overdue), and those coming up in the next week. Each plant entry shows its name,
species, and location in the home. Plants that need water now have a quick-action button to log
that they were watered. Overdue plants are visually distinguished. The user can navigate to all
plants or add a new one from this screen.

SCREEN 2 — Add Plant
A form screen where the user registers a new plant. The user provides a nickname, selects the
plant species from a searchable list, sets how often it needs water, picks its location in the
home, and sets the last watered date. When a species is selected the app suggests a watering
interval automatically. The user can optionally add a photo of the plant. A save action
completes the form.

SCREEN 3 — Empty state
What the dashboard looks like when the user has no plants yet. It should feel welcoming and
guide the user to add their first plant.
```

---

## v1.0 — Pending ⬜

> Plant detail, watering history, notifications banner, settings.
> Use exactly the same visual style as MVP screens — same typography, color palette, surface layering, navigation bar, card style, and component language.

```
Design additional screens for PlantPal, a mobile app for tracking indoor plant watering schedules.

These screens extend the MVP. Use exactly the same visual style as the existing MVP screens —
same typography, color palette, surface layering, navigation bar, card style, and component
language. A user switching between MVP and v1.0 screens should feel no visual discontinuity.

Design 3 screens:

SCREEN 1 — Plant detail
When the user taps a plant from the dashboard they land here. This screen is the home for
everything about one plant. It shows the plant's photo prominently, its name, species, location,
and the next scheduled watering date. There is a button to log that the plant was watered right
now. Below the plant information is a history of every past watering, listed in reverse
chronological order — each entry shows the date and time. The user can also reach the edit form
and delete the plant from this screen.

SCREEN 2 — Dashboard with permission warning
The dashboard from the MVP, in a state where the user has denied notification permissions. A
non-blocking warning appears that tells the user reminders are off and gives them a way to enable
notifications in device settings. The warning should not prevent access to any other part of the
screen — everything below still works normally. Also show a second variant where the warning is
about photo library access being denied instead.

SCREEN 3 — Settings
A screen with two functional sections. The first is about device permissions: it shows whether
notification access and photo library access are granted or denied, and for each one that is
denied, provides a way to open the device settings to fix it. The second section is about user
preferences: a time picker for when daily reminders should fire, and a toggle to enable or
disable weather-aware watering (which postpones reminders for outdoor plants after rainfall).
```

---

## v2.0 — Pending ⬜

> Claude advisor chat, weather badges, cloud sync status.
> Use exactly the same visual style as the MVP screens.

```
Design advanced feature screens for PlantPal, a mobile app for tracking indoor plant watering
schedules.

These screens introduce the three major v2.0 features. Use exactly the same visual style as
the MVP screens — same typography, palette, surface system, navigation bar, and card components.
The features are new but the app should feel like one continuous product.

Design 3 screens:

SCREEN 1 — Plant advisor
A conversational screen where the user can ask care questions about a specific plant. The
currently selected plant is shown at the top so the user knows which plant the conversation is
about. The main area shows the conversation history — the user's questions and the AI's responses.
Common care questions are available as quick-tap shortcuts so the user doesn't have to type from
scratch. There is a text input at the bottom to type a free-form question. If the user hasn't
set up an API key yet, the screen explains what's needed and how to add it in settings.

SCREEN 2 — Dashboard with weather adjustment
The same dashboard as the MVP, but some plants — specifically those in outdoor locations like
a balcony or garden — show a visual indicator that their next watering has been automatically
postponed because of recent rainfall. The user should be able to see at a glance which plants
were adjusted and understand why without needing to tap into anything.

SCREEN 3 — Account and sync
A screen showing the user's account information (signed in via Google) and the sync status of
their data. It shows when the data was last successfully synced, which devices are connected to
the account, and a button to trigger a sync manually. It also handles the offline state — when
the device has no internet connection, the screen reassures the user that their local changes are
safe and will sync automatically when connectivity returns.
```
