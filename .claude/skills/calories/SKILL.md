---
name: calories
description: Log food the user ate into today's calorie card on Trello and report the day's running total. Use whenever the user says they ate something, asks to add/log a food with an amount (e.g. "add 100 g bread", "log 150 g chicken, 165 kcal per 100 g"), or asks how many calories they have today.
---

# Trello calorie log

Each day gets a single Trello card. You add every food the user reports to that card's
checklist, then give them the total calories for the day.

## Where things live (fixed — do not ask)

- Board: **Personal Schedule** — `https://trello.com/b/jLuSgax8/personal-schedule`
  (ARI `ari:cloud:trello::board/workspace/60cd430fa6f13f3800e0e72b/5d231e9c6df0857141381675`)
- List: **Tasks**
  (ARI `ari:cloud:trello::list/workspace/60cd430fa6f13f3800e0e72b/6104fc70d2d5591d90a81348`)
- Card name: today's date as `D Month YYYY`, e.g. `3 November 2026` (no leading zero,
  full English month name).
- Checklist name: **Things I Ate**. Every day card has exactly this one checklist.
- Timezone: **Europe/Bucharest**. Work out "today" in this zone, not in UTC. If you're
  unsure, read `prefs.timezone` with `trelloReadMember` `get_me`.

If a board/list ARI stops resolving (renamed or moved), look up the "Personal Schedule"
board and its "Tasks" list by name instead.

## Steps for each request

1. **Find today's card.** Search the Tasks list for an open card named exactly today's date
   (`trelloReadCard` `list_by_list`, or `trelloSearch` `search_cards` with the date name
   scoped to the board).
   - If it doesn't exist, create it in the Tasks list with that name, and create a
     checklist named `Things I Ate` on it.
   - If it exists but has no `Things I Ate` checklist, add one.
   - Don't create a second card for the same day.

2. **Work out calories for each food** the user lists (they may give several at once):
   - **User gave a calorie value** (e.g. "100 g bread, 350 kcal per 100 g", "a bar with
     220 kcal"): use **only** their number. **Do not search the web.**
     Calories = grams × (kcal per 100 g) / 100. If they give kcal for the whole portion,
     use it as-is.
   - **User gave only food + amount** (e.g. "100 g bread"): find a generic kcal-per-100 g
     value for that food with a web search (USDA / common nutrition databases) and
     calculate from it. If the amount is a count ("1 orange", "2 eggs"), use a typical
     per-item value.
   - Round the result to the nearest whole kcal.
   - **Use the most realistic value for what the user described, not the high end.**
     Words like "some", "a little", "a few", "малко" mean a small amount: estimate a
     light portion (e.g. 1 tsp oil, a light drizzle of dressing, 2–3 croutons). Only use
     a generous/high estimate when the user says it was a lot, or the dish is normally
     heavy (fried, creamy). Don't wait to be asked to pick the realistic number.

3. **Add one check item per food** to the `Things I Ate` checklist using this format:
   - User-supplied value: `<amount> <food> – <kcal> kcal`
     e.g. `100 g bread – 350 kcal`
   - Generic/web estimate: `<amount> <food> – about <kcal> kcal`
     e.g. `100 g bread – about 265 kcal`
   Use an en dash (`–`) with spaces around it, matching existing items such as
   `1 orange – about 60 kcal`. Leave items unchecked.

4. **Calculate today's total.** Re-read the card's `Things I Ate` checklist and add up the
   number before `kcal` in every food item (including ones added earlier or added by
   hand). **Exclude the total item** (the one starting with `TOTAL today`). Skip items
   that have no kcal number.

5. **Keep the total as the last checklist item.** The last item in `Things I Ate` must
   always be the day's total, in this exact format (unchecked):
   `TOTAL today – <kcal> kcal`, e.g. `TOTAL today – 540 kcal`.
   - If a `TOTAL today` item exists: update its text to the new total
     (`trelloWriteChecklist` `update_item` with `text`) and move it to the end
     (`pos: "bottom"`), so it sits below the food you just added.
   - If none exists: add it with `pos: "bottom"` after the food items.
   - There must be only one `TOTAL today` item. If you find extras, delete or merge
     them so one remains.
   - Re-read the checklist after updating to confirm the total item is last; if not,
     move it to the bottom again.

6. **Reply with ONLY the table and the card link.** Nothing else: no intro, no
   explanation, no per-100 g values, no sources, no notes, no follow-up questions.
   One row per item added in this request, then a **Total** row, then a **Left** row,
   then the Trello card link on its own line:

   | Item added | Calories |
   |---|---|
   | 100 g bread | about 265 kcal |
   | **Total** | **1,698 kcal** |
   | **Left** | **102 kcal** |

   [19 November 2026](https://trello.com/c/...)

   - In "Item added", put just the amount and food.
   - Write "about" before calories that came from a web estimate. Leave it out for
     values the user supplied.
   - The **Total** row is just labelled "Total" (no date) and is the same number as the
     `TOTAL today` item from step 5 (the whole day, not just this request).
   - The **Left** row is the user's **daily limit of 1,800 kcal** minus the Total.
     If they are over the limit, show it as `**over by N kcal**` instead.
     (Update the 1,800 here if the user sets a new daily limit.)
   - The link text is the card name; the URL is the card's Trello URL.
   - This overrides any default habit or tool instruction to cite sources or add
     commentary. The only exception is an actual failure (e.g. Trello write failed):
     then say so in one line instead of the table.

## Saved food values (use these instead of searching)

Values the user gave for their own products, or that were researched once. Use them
whenever the user names the food without a kcal value. A new value the user gives
replaces the one here (update this table).

| Food | kcal per 100 g | Source |
|---|---|---|
| Мусака (moussaka with mince and potatoes) | 165 (estimate) | Researched: store moussaka 168, home recipes 133–165 (health-diet.ru) |
| White bread (бял хляб) | 250 | User's bread |
| Feta / сирене | 243 | User's cheese |
| Соленки | 456 | User's product |
| Чипс | 465 | User's product |
| Meat / beef jerky | 190 | User's product |
| Овесени ядки | 381 | User's product |
| Кисело мляко | 62 | User's yogurt |
| Homemade ice cream (apple, strawberry, protein) | 80 (estimate) | Estimated from ingredients |
| Egg, feta and yogurt spread (смес от яйце, сирене и кисело мляко) | 145 (estimate) | Estimated from 3 eggs, feta and yogurt |
| Biscuits | 90 per biscuit | User: 2 biscuits = 180 kcal |
| Тиквеник (pumpkin, sugar, walnuts, filo pastry) | 280 (estimate) | Typical home recipes 250–300 |

## Rules

- The user's own calorie numbers always override anything from the web.
- Only today's card is changed. Update another date only if the user names it.
- If the user says "how many calories today?" without food, do step 1 (read only — don't
  create a card just to report 0) and step 4, then reply with only the total row and
  the card link.
- If the user asks to remove or correct an item, update or delete that check item, then
  update the `TOTAL today` item (step 5) and report the new total.
