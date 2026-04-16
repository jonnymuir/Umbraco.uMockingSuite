# Decision: Save Detection Approach for uMockingSuite

**Date:** 2026-04-17  
**Author:** Theresa (UI/Frontend Expert)  
**File affected:** `uMockingSuite/App_Plugins/uMockingSuite/umockingsuite-workspace-context.js`

## Problem

In Umbraco 17.3.4, the `isSubmitting` observable on `UmbDocumentWorkspaceContext` emits `undefined` once on load and then never again. The save button does not update this observable, making it useless as a save detector in its original form.

## Decision

Use **two complementary strategies**:

### PRIMARY: Wrap `requestSubmit` on the context instance

```javascript
const originalRequestSubmit = context.requestSubmit.bind(context);
context.requestSubmit = async (...args) => {
    await originalRequestSubmit(...args);
    this.#handleSaveCompleted();
};
```

The save button in Umbraco 17 calls `requestSubmit()` on the document workspace context. Monkey-patching the instance method lets us intercept saves reliably without any private API access. We await the original to ensure the save has fully resolved before triggering our handler.

### SECONDARY: Fixed `isSubmitting` observer (fallback)

Keep observing `isSubmitting` but:
- Skip `undefined` emissions (the initial load emission)
- Only fire on a strict `true → false` transition
- Initialise `#wasSubmitting = undefined` (not `false`) so the first real emission doesn't false-trigger

## Additional changes

- `#isFetchingMockMessage` guard in `#handleSaveCompleted` prevents double-firing if both strategies happen to trigger simultaneously (e.g. if Umbraco fixes `isSubmitting` in a later release)
- `finally` block resets the guard so subsequent saves always work
- Prototype chain property enumeration logged on context acquisition for ongoing diagnostics

## Status

Implemented and deployed. Both source and `Umbraco.AI.Demo/App_Plugins` copies updated.
