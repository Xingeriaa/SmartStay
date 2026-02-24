---
description: Perform a full deep consistency audit of the BMS system based on provided lists.
---

Perform a FULL STRICT AUDIT of my BMS system.

Input may include:

- function list
- database schema
- use cases
- APIs
- workflows
- controllers
- models

You MUST analyze deeply and systematically.

---

## STEP 1 — STRUCTURE VALIDATION

Check alignment between:

- Functions ↔ Use Cases
- Use Cases ↔ Database
- Database ↔ APIs
- APIs ↔ Controllers

Report any mismatch.

---

## STEP 2 — DATABASE INTEGRITY CHECK

Verify:

- missing foreign keys
- missing unique constraints
- snapshot correctness
- soft delete consistency
- audit coverage
- financial ledger safety

Flag ALL risks.

---

## STEP 3 — BUSINESS RULE VALIDATION

Strictly verify:

- one active contract per room
- meter reading monotonic increase
- invoice calculation correctness
- deposit liability handling
- tenant balance ledger enforcement

If ANY rule is weak → flag as CRITICAL.

---

## STEP 4 — RBAC SECURITY CHECK

Detect:

- privilege escalation risks
- missing scope checks
- tenant data leakage risks
- staff over-permission

Be extremely strict.

---

## STEP 5 — PRODUCTION READINESS SCORE

Give scores:

- Data integrity (0–100)
- Financial safety (0–100)
- RBAC security (0–100)
- Overall production readiness (0–100)

---

## STEP 6 — REQUIRED FIX LIST

Output MUST include:

🔥 Critical fixes  
⚠️ Important improvements  
💡 Optional optimizations

Each fix must be:

- concrete
- actionable
- minimal ambiguity

---

## OUTPUT STYLE

- No fluff
- No praise
- No repetition
- Focus on defects and hardening
- Think like a senior system architect
