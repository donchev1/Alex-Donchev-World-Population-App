---
name: add-workout
description: Log a workout to today's Trello day card in a "Workouts" checklist. Use whenever the user asks to add/log a workout or exercise (e.g. "add workout walking 30 min", "log chest workout", "add running 5 km", "triceps workout").
---

# Add workout

Workouts go on the same daily Trello card the `calories` skill uses, but in their own
checklist called **Workouts**. Never touch the `Things I Ate` checklist from this skill.

## Where things live (fixed — do not ask)

- Board: **Personal Schedule** — `https://trello.com/b/jLuSgax8/personal-schedule`
  (ARI `ari:cloud:trello::board/workspace/60cd430fa6f13f3800e0e72b/5d231e9c6df0857141381675`)
- List: **Tasks**
  (ARI `ari:cloud:trello::list/workspace/60cd430fa6f13f3800e0e72b/6104fc70d2d5591d90a81348`)
- Card name: today's date as `D Month YYYY`, e.g. `3 November 2026` (no leading zero,
  full English month name).
- Checklist name: **Workouts**.
- Timezone: **Europe/Bucharest**. Work out "today" in this zone, not in UTC.

If a board/list ARI stops resolving, look up the "Personal Schedule" board and its
"Tasks" list by name instead.

## Steps

1. **Find today's card** in the Tasks list (an open card named exactly today's date).
   - If it doesn't exist, create it in the Tasks list with that name, and also create the
     `Things I Ate` checklist on it (so the `calories` skill finds the card set up the
     usual way). Don't create a second card for the same day.

2. **Find the `Workouts` checklist** on that card. If it doesn't exist, create it.

3. **Add one check item per workout** the user mentions, written as they said it, with
   tidy capitalisation, e.g. `Walking – 30 min`, `Running – 5 km`, `Chest workout`,
   `Triceps workout`. Include duration, distance, sets/reps or anything else they give;
   don't invent details they didn't give. No calorie calculations, no web searches.
   Use an en dash (`–`) with spaces when adding a detail. Leave items unchecked.

4. **Reply with ONLY a short table and the card link.** No intro, no commentary:

   | Workout added |
   |---|
   | Walking – 30 min |

   [3 November 2026](https://trello.com/c/...)

   If something fails (e.g. a Trello write), say so in one line instead.

## Rules

- Only today's card is changed, unless the user names another date.
- If the user asks to remove or correct a workout, update or delete that check item in
  `Workouts` and reply with the same table format showing the change.
