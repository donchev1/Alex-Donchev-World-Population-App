---
name: weekly-report
description: Build the user's weekly goals report (HTML page) from their Trello day cards - goal progress for running, walking, training and game dev, calories, and weight vs the 83 kg goal - and move past weeks' day cards from Tasks to Achievements. Use when the user asks for "the report", "weekly report", "репорт", "отчет за седмицата", how they did against their goals this week, or to tidy last week's cards.
---

# Weekly goals report

A weekly HTML report of how the user did against their goals, built from their Trello
day cards and published as an Artifact page. Each run also tidies Trello: day cards from
earlier weeks move from **Tasks** to **Achievements**.

The goals below stay the same every week **until the user changes them**. When they do,
update the table here (and commit the skill).

## The user's weekly goals (Monday–Sunday)

| Goal | Target | How to measure |
|---|---|---|
| Run | **2 × 5 km** | Count running items in `Workouts`; % = runs ÷ 2 (or km ÷ 10) |
| Walk | **20,000 steps** in the week | Sum steps of walking items; km-only walks × 1,520 steps/km |
| Train | **7 muscle groups**: abs, chest, triceps, biceps, back, legs, shoulders | Each group counts once if trained any day (squats = legs) |
| Game dev | **5 hours** | Sum hours in the `Game Dev` checklists |

**Overall score** = average of the four goal percentages, **not capped at 100%** (the user
wants going over a goal to count, e.g. 136% walking).

## Weight goal

- Goal: **83.0 kg by 31 December 2026**.
- Readings so far: 87.9 kg (Mon 2 Nov), 87.0 kg (Mon 9 Nov), 87.1 kg (Thu 12 Nov),
  86.8 kg (Mon 16 Nov). Add new readings here when the user gives them.
- Original plan from 9 Nov: about 540 g a week. Monday targets:
  16 Nov 86.5 · 23 Nov 85.9 · 30 Nov 85.4 · 7 Dec 84.8 · 14 Dec 84.3 · 21 Dec 83.8 ·
  28 Dec 83.2 · 31 Dec 83.0.
- On 16 Nov the user was 0.3 kg behind (86.8 vs 86.5). Required pace from 86.8 kg is now
  about **590 g a week** (84 g a day, ~650 kcal/day deficit). If the user is behind,
  recalculate from the latest weight.
- User profile: 33 years, 175 cm. Estimated maintenance about 2,500–2,800 kcal/day.
- Remind them that day-to-day swings of 0.5–2 kg are water/salt; compare Monday weigh-ins.

## Where the data is

- Trello board **Personal Schedule**, list **Tasks** (and **Achievements** for past weeks).
  Day cards are named `D Month YYYY`. Timezone Europe/Bucharest.
- Checklists on each day card:
  - `Things I Ate` – last item `TOTAL today – N kcal` is the day's calories.
  - `Workouts` – items end with `– about N kcal` (calories burned).
  - `Game Dev` – items like `Game dev – 1 h`.
  - `Not eaten (info only)` – ignore.
- Read every day card of the week (Mon–Sun) with `trelloReadChecklist` `list_by_card`.
  Calculate with a script; don't add up by hand.

## Report layout (same as previous weeks)

Write one self-contained HTML page (light + dark theme, phone-friendly), in this order:
1. Header: ring with the overall %, week dates, one-sentence summary.
2. Four goal cards (run, walk, train, game dev) with %, detail line, bar, "✓ Goal hit"
   badge or what's missing.
3. Training: the 7 muscle groups, each with the day it was trained (or "Not done").
4. Steps: a bar per day that had walks.
5. Day by day: each day's calories and chips for runs, walks, training, game dev.
6. Calories: daily average, week total, workout calories burned, change vs last week's
   average, bars per day. Mark an unfinished day ("so far").
7. Weight: latest, change, goal, still to lose, status pill, table of Monday targets vs
   actual readings, note on daily swings.
8. Next week / still open: what's missing.
9. Footer: where the data came from and how the score is calculated.

Publish a new Artifact each week, titled `Weekly Goals D–D Mon` (e.g. `Weekly Goals 9–15 Nov`).

## Move past weeks' cards to Achievements (every time this skill runs)

1. List the open cards in **Tasks** (`trelloReadCard` `list_by_list`,
   ARI `ari:cloud:trello::list/workspace/60cd430fa6f13f3800e0e72b/6104fc70d2d5591d90a81348`).
2. Pick only cards whose name is exactly a date `D Month YYYY` and that are **before this
   week's Monday** (Europe/Bucharest). Cards for the current Mon–Sun week stay in Tasks.
   If the report is for a week that has just ended (run on Sunday night or later), that
   week's cards move too once the report is published.
3. Move each one to **Achievements**
   (ARI `ari:cloud:trello::list/workspace/60cd430fa6f13f3800e0e72b/63bbafd95b7d2a01bfd9dca8`)
   with `trelloWriteCard` `move`, oldest first, using increasing `pos` values (e.g.
   900000000 + day number) so they stay in date order at the bottom.
4. Never move, edit or archive any other card (e.g. "To buy", "Memories").
5. Mention in the reply how many cards moved.

Moved on 22 Nov 2026: 4–15 Nov (3 Nov was already gone from Tasks).

## Past reports

| Week | Overall | Link |
|---|---|---|
| 2–8 Nov 2026 | 109% (run 100, walk 136, train 100, game dev 100) | https://claude.ai/artifact/4kzZkmKX3y96hBGaCEvAd9 |
| 9–15 Nov 2026 | 85% (run 100, walk 101, train 100, game dev 40) | https://claude.ai/artifact/UNJH8zVWPmSbyiNEWBbUsj |

Add each new week to this table. Average calories: 2–8 Nov 1,736 kcal/day; 9–15 Nov 1,923 kcal/day.
