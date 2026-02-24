---
trigger: always_on
---

You are working on the BMS (Building Management System).

This is a production-grade property management platform with strict financial and RBAC requirements.

You MUST follow ALL rules below permanently.

---

## I. ARCHITECTURE AWARENESS

The system includes:

- ASP.NET Core MVC Web App
- REST API backend
- SQL Server database
- RBAC permission model
- Financial ledger integrity

NEVER generate code that violates this architecture.

---

## II. DATA CONSISTENCY RULES (CRITICAL)

You MUST always ensure:

1. Snapshot Integrity
   - InvoiceDetails.UnitPriceSnapshot MUST come from ContractServices
   - NEVER read price from Services when generating invoices

2. Meter Reading Safety
   - NewElectricityIndex >= OldElectricityIndex
   - NewWaterIndex >= OldWaterIndex
   - UNIQUE(RoomId, MonthYear) must be respected

3. Contract Safety
   - One room can have ONLY ONE active contract
   - Contract extension MUST NOT modify original contract

4. Tenant Balance Ledger
   - ANY change to TenantBalances MUST create TenantBalanceTransactions
   - NEVER update balance without ledger record

---

## III. RBAC ENFORCEMENT

Always respect role scope:

Admin:

- system configuration
- user & permission management
- audit logs
- global reports

Staff:

- operational management
- buildings, rooms, contracts, invoices
- cannot override financial history

Tenant:

- self-service only
- own invoices
- own tickets
- own profile

NEVER generate code that bypasses RBAC.

---

## IV. SOFT DELETE POLICY

Business tables MUST use soft delete.

Rules:

- Use IsDeleted = 1 instead of physical delete
- All queries MUST filter IsDeleted = 0
- NEVER hard delete financial data

---

## V. AUDIT LOG REQUIREMENT

The following operations MUST generate AuditLogs:

- contract changes
- price changes
- invoice changes
- payment confirmation
- liquidation

If missing → you MUST flag it as an error.

---

## VI. FINANCIAL SAFETY RULES

Deposit is a LIABILITY, not revenue.

Invoice formula MUST be:

TotalAmount =
Rent

- Services
- Electricity
- Water
- PreviousDebt

* CreditBalance

Never allow manual override of calculated totals.

---

## VII. WHEN ANALYZING USER DATA

When the user provides:

- function list
- database schema
- use cases
- APIs
- workflows

You MUST:

1. Cross-check relationships
2. Detect logical conflicts
3. Detect missing constraints
4. Detect financial risks
5. Detect RBAC gaps
6. Detect data integrity risks

Be strict and precise.

No fluff.
No generic advice.
Only actionable findings.

---

## VIII. RESPONSE STYLE

Tone must be:

- serious
- concise
- technical
- direct

When issues exist, you MUST:

- name the problem
- explain the risk
- propose the exact fix

Always prioritize production safety.
