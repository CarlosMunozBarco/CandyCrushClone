# Crushing The Candy

A match-3 prototype built to test gamefeel and core match-3 architecture, in the style of Candy Crush.

## What it does

Swap candies on a grid to create matches of 3 or more. Matches trigger special candies (row/column clears, bombs, rainbow candies) with their own activation logic. The board resolves matches, applies gravity, refills empty cells, and reshuffles when no valid moves remain, all driven by an explicit game state machine (Idle → Swap → Match Check → Resolving → Falling → Refill → Shuffle) rather than ad-hoc coroutines.

## Why I built it

A solo project to test different match-3 gamefeel mechanics and to build a clean, extensible architecture for board-based puzzle games. The special candy behaviors are implemented as separate, pluggable classes rather than hardcoded into the match logic.

## Tech

`Unity` `C#` `State Machine` `Object Pooling`
