---
name: trello-calorie-log
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

3. **Add one check item per food** to the `Things I Ate` checklist using this format:
   - User-supplied value: `<amount> <food> – <kcal> kcal`
     e.g. `100 g bread – 350 kcal`
   - Generic/web estimate: `<amount> <food> – about <kcal> kcal`
     e.g. `100 g bread – about 265 kcal`
   Use an en dash (`–`) with spaces around it, matching existing items such as
   `1 orange – about 60 kcal`. Leave items unchecked.

4. **Calculate today's total.** Re-read the card's `Things I Ate` checklist and add up the
   number before `kcal` in every item (including ones added earlier or added by hand).
   Skip items that have no kcal number and mention them.

5. **Always reply with a simple table** after every add. Do this every time, even for
   a single item. Use exactly these columns, one row per item added in this request,
   then a final **Total today** row:

   | Item added | Calories |
   |---|---|
   | 100 g bread (about 265 kcal/100 g) | about 265 kcal |
   | **Total today (3 November 2026)** | **325 kcal** |

   - In "Item added", put the amount and food. For web estimates, add the per-100 g
     value you used in brackets so the user can correct it.
   - Write "about" before calories that came from a web estimate. Leave it out for
     values the user supplied.
   - The total row is the sum from step 4 (the whole day, not just this request).
   - Keep any text outside the table to one short line at most: the card link the first
     time you create a card each day, items skipped in the total, and a "Sources:" line
     when you used a web search.

## Rules

- The user's own calorie numbers always override anything from the web.
- Only today's card is changed. Update another date only if the user names it.
- If the user says "how many calories today?" without food, do step 1 (read only — don't
  create a card just to report 0) and step 4.
- If the user asks to remove or correct an item, update or delete that check item, then
  report the new total.
