---
name: Do not dismiss Windows rendering issues as "expected behaviour"
description: User is experienced with MAUI cross-platform rendering — never explain differences away, fix them
type: feedback
---

Do not say Windows renders differently and that it is "expected behaviour". Fix it.

**Why:** User explicitly corrected this — "I know how the navigation on windows works and get rendered. If i say i do not see it, means it is not there. The benefit of maui is that it works on all 3 platforms." The issue was a real bug (bad color values), not a platform difference.

**How to apply:** When the user reports something missing or broken on Windows (or any platform), investigate and fix it. Do not preemptively explain it as a known platform rendering difference unless you have verified the code is correct and it truly is a platform limitation with no workaround.
