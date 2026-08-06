# Submission write-up

Copy this file, fill it in, and submit it alongside your code. Short,
specific answers are better than long, general ones.

## 1. The gaps you filled in

The brief left three things unspecified: required vs. optional fields,
whether repeat links are reused or reissued, and the shape of the
shareable ID. Walk us through the calls you made on each, and why.

_Your answer:_

**Required vs optional:** `name`, `headline`, and `skills` are all required. Empty or whitespace-only values return `400 Bad Request`. Skills must contain at least one non-empty item after trimming. I kept optional fields out of scope because the brief already defines the minimum profile shape, and adding optional fields would increase API branching without evaluation value.

**Repeat links:** Every `POST /api/profiles` creates a new profile with a new ID. I rejected content-based deduplication because there is no user identity or authentication to define "same profile", and hash reuse could incorrectly merge two different people with similar data.

**Shareable ID shape:** I use a `Guid` string (e.g. `3fa85f64-5717-4562-b3fc-2c963f66afa6`). It is built into .NET, requires no database sequence, and is much harder to guess than sequential integers. I rejected short slugs for now because they need extra collision handling and encoding logic beyond the core scope.

## 2. What's next

If you had one more day on this, what's the first thing you'd do, and
why that thing first?

_Your answer:_

Add file-based JSON persistence behind `IProfileRepository`. It would survive process restarts with minimal complexity, still avoids a real database, and demonstrates that the repository abstraction is useful — without changing controllers or services.

## 3. AI tools

Did you use AI tools while building this (Copilot, ChatGPT, Claude,
etc.)? If yes — what did you use them for, and was there anything an AI
suggested that you changed or rejected, and why? If no, that's fine —
just say so.

_Your answer:_

Yes — Cursor (Claude) for planning, architecture review, and implementation scaffolding. I used it to draft the initial PRD and compare trade-offs, then reviewed and tightened decisions before coding.

I rejected suggestions that would over-engineer the case: Clean Architecture folder structure, FluentValidation as a dependency, short URL slugs instead of GUIDs, and content-hash link reuse. I kept the solution aligned with the brief's intentionally small scope while still showing professional structure (service + repository + focused unit tests).

## 4. Optional — skip freely, no penalty either way

If Humanas had to onboard 10,000 new profiles overnight and only had
capacity to manually review 1% of them, how would you decide which 1%?
There's no single right answer here — we're curious how you think, not
what you conclude.

_Your answer:_

I would prioritize review by risk signals, not random sampling. First pass: flag profiles with suspicious patterns (duplicate content clusters, extreme skill counts, gibberish text, rapid bulk creation from the same source). Second pass: sample across segments (new vs returning patterns, different skill domains) to catch edge cases. The 1% manual review budget goes to highest-risk and highest-uncertainty cases; the rest get automated checks and deferred audit. The goal is to catch abuse and quality issues early without pretending 100 profiles can represent 10,000 fairly.
