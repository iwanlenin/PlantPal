---
name: Explain every command before running it
description: User requires an explanation of what each command does before it is executed
type: feedback
---

Always explain what a bash command does before running it — purpose, what it changes, and why it is needed at that point in the workflow. Never run a command silently or without context.

**Why:** User explicitly requested this during the PlantPal setup phase. They want to understand each step, not just see results.

**How to apply:** For every Bash tool call, write a plain-English explanation first. This applies to git commands, CLI tools, curl, winget, and any other shell invocation.
