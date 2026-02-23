# GOAP CORE

This folder contains a lightweight Goal Oriented Action Planning implementation used by Heroes. The system is built around immutable-style state transitions and a planner that searches for a sequence of actions to satisfy the most important unmet goal.

The world model is `AgentState`, a value type that stores beliefs as a float array indexed by integer IDs. It supports cloning, value equality, and hashing so states can be compared and cached. `AgentContext` wraps an `AgentState` and always clones on construction, which keeps planning isolated from the live agent data. `AgentStateExtensions` provide small utilities to evolve states in a functional way. `Mutate` applies a ref mutator and returns a new state, while `Clamp` and `Bucket` normalize belief values to a range or a discrete step.

Goals are described by `Goal`. A goal has a priority, a name/description, and three delegates: `Importance` for dynamic scoring, `Achieved` to verify completion, and `Heuristic` to estimate remaining cost. The planner uses `Importance * Priority` to rank candidate goals and will skip any goals already achieved in the current context. Actions are described by `Action`, which binds a name/description with `PreConditions`, `Effect`, `Time`, and an `Implementation` delegate. The `Effect` delegate is expected to be pure and return the next `AgentState`, while `Implementation` is where you perform the real gameplay side effects once a plan is chosen.

`Planner` performs an iterative deepening search over actions with a cost cutoff derived from the goal heuristic. It keeps a stack of states, costs, and chosen actions, and returns the first plan that reaches the goal within the depth and cost limits. A `TranspositionTable` caches the best known cost for a state and prevents revisiting states when the new path is not cheaper. This helps control the branching factor without enforcing strict action ordering.

`Archetype` is a thin container that holds a set of actions and goals and exposes a single `Plan` call. It allows each agent archetype to reuse the same planner with a tailored list of available actions and objectives.

Typical flow is to build actions and goals with their builders, construct an `Archetype`, and then call `Plan` with the current `AgentContext`. The returned list of actions can be executed in order, applying each action’s `Implementation` while using `Effect` to predict state transitions during planning.
