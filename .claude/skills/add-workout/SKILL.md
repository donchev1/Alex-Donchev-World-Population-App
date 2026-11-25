---
name: add-workout
description: Log a workout to today's Trello day card in a "Workouts" checklist, with calories burned, and report today's and this week's totals (calories burned, steps walked, km run). Use whenever the user asks to add/log a workout or exercise (e.g. "add workout walking 30 min", "log chest workout", "add running 5 km", "triceps workout", "add workout 3000 steps").
---

# Add workout

Workouts go on the same daily Trello card the `calories` skill uses, but in their own
checklist called **Workouts**. Never touch the `Things I Ate` checklist from this skill.

## Where things live (fixed — do not ask)

- Board: **Personal Schedule** — `https://trello.com/b/jLuSgax8/personal-schedule`
  (ARI `ari:cloud:trello::board/workspace/60cd430fa6f13f3800e0e72b/5d231e9c6df0857141381675`)
- List: **Tasks**
  (ARI `ari:cloud:trello::list/workspace/60cd430fa6f13f3800e0e72b/6104fc70d2d5591d90a81348`)
- Past weeks' day cards may have been moved to the **Achievements** list on the same board
  (ARI `ari:cloud:trello::list/workspace/60cd430fa6f13f3800e0e72b/63bbafd95b7d2a01bfd9dca8`).
  Look there too when you need an earlier day of this week.
- Card name: the date as `D Month YYYY`, e.g. `3 November 2026` (no leading zero,
  full English month name).
- Checklist name: **Workouts**.
- Timezone: **Europe/Bucharest**. Work out "today" in this zone, not in UTC.
- Week: **Monday to Sunday**.

If a board/list ARI stops resolving, look up the "Personal Schedule" board and its
"Tasks" / "Achievements" lists by name instead.

## User profile (for calorie estimates)

- Age 33, male, height 175 cm, weight **87 kg** (update this if the user gives a new weight).
- Walking pace: about **1,505 steps per km** (user's treadmill: 6,000 steps = 3.99 km, 25 Nov 2026).

## Calories burned — fixed rates (do not search the web each time)

Researched once from MET values (Compendium of Physical Activities, via calculator sites)
for an 87 kg person. Formula: kcal = MET × kg × hours.

| Activity | Rate for 87 kg | Basis |
|---|---|---|
| Walking / treadmill walking (~5 km/h) | **0.042 kcal per step** ≈ 64 kcal per km ≈ 5.3 kcal per min | MET 3.5 |
| Running / treadmill running | **87 kcal per km** | ~1 kcal per kg per km (≈ MET 8–10 at 8–10 km/h) |
| Strength training (chest, back, legs, arms, abs, shoulders…) | **5.3 kcal per min**; if no duration is given, assume **30 min = 160 kcal per muscle-group item** | MET 3.5 (general resistance training) |
| Anything else (cycling, swimming, sports…) | Use a standard MET value × 87 kg × hours; if no duration, ask | Compendium MET |

Rules:
- **If the user gives the calories burned** (e.g. "225 calories"), use their number, not the rate.
- Walking given only in km or time: convert with the rates above. Walking given only in
  km: steps = km × 1,505 (for the weekly step total).
- Running given only in time: assume about 9 km/h to get km.
- If the user's weight changes, rescale: walking kcal/step = 0.00048 × kg, running kcal/km = kg,
  strength kcal/min = 3.5 × 3.5 × kg / 200.
- Round each workout to the nearest whole kcal.

## Steps

1. **Find today's card** in the Tasks list (an open card named exactly today's date).
   - If it doesn't exist, create it in the Tasks list with that name, and also create the
     `Things I Ate` checklist on it (so the `calories` skill finds the card set up the
     usual way). Don't create a second card for the same day.

2. **Find the `Workouts` checklist** on that card. If it doesn't exist, create it.

3. **Add one check item per workout** the user mentions, with its calories at the end:
   `<Workout> – <details> – about <kcal> kcal` (or `– <kcal> kcal` without "about" when the
   user gave the number). Examples:
   - `Walking – 2523 steps – about 106 kcal`
   - `Running – 5 km – about 435 kcal`
   - `Chest workout – about 160 kcal`
   - `Walking – 5000 steps, 55 min, 3.5 km – 225 kcal`
   Include duration, distance, sets/reps or anything else they give; don't invent details.
   Use an en dash (`–`) with spaces. Leave items unchecked.

4. **Work out the totals** by reading the `Workouts` checklists of every day card from this
   week's Monday up to today (Tasks list, plus Achievements if a card was moved):
   - **Calories burned today** and **this week**: sum the kcal at the end of each item.
     If an older item has no kcal, calculate it with the rates above and update that item's
     text so it has one.
   - **Km walked today** and **this week**: sum the km of all walking items. If an item
     has only steps, convert with 1,505 steps per km. (The user tracks walking in **km**, not
     steps, since 25 Nov 2026. Weekly walking goal: **29 km**.)
   - **Km run today** and **this week**: sum the km of all running items.

5. **Reply with ONLY this table and the card link.** No intro, no commentary. Always
   include every row below (show 0 when there is nothing yet), with today's rows first:

   | Workout added | Calories burned |
   |---|---|
   | Walking – 5038 steps | about 212 kcal |
   | **Burned today (11 November 2026)** | **1,094 kcal** |
   | **Walked today** | **5.3 km** |
   | **Running today** | **5 km** |
   | **Burned this week (9–15 Nov)** | **2,104 kcal** |
   | **Walked this week** | **9.3 of 29 km** |
   | **Running this week** | **10 km** |

   [11 November 2026](https://trello.com/c/...)

   Reply in the language the user wrote in (Bulgarian or English). If something fails
   (e.g. a Trello write), say so in one line instead.

## Rules

- Only today's card is changed, unless the user names another date (then update that day's
  card, and still report today's and this week's totals).
- If the user asks to remove or correct a workout, update that check item in `Workouts`
  (rename it if it can't be deleted) and reply with the same table showing the change.
- Game dev hours go in a separate `Game Dev` checklist on the day card, not in `Workouts`,
  and have no calories.
