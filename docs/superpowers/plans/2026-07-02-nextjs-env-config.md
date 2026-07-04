# Environment Configuration Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Implement centralized environment variable configuration matching `workflow/nextjs-env-config-workflow.md` adapted for the CRA React SPA frontend.

**Architecture:** Central config mapping dynamic environmental variables with safe fallback values, resolving paths via `src/config/index.ts`.

**Tech Stack:** React, TypeScript, Environment Variables.

---

### Task 1: Initialize Environment Files and Update Gitignore

**Files:**
- Create/Overwrite: `cms.frontend/.env.development`
- Create/Overwrite: `cms.frontend/.env.production`
- Modify: `cms.frontend/.gitignore`

**Interfaces:**
- Produces: Environment settings for Dev and Production environments.

- [ ] **Step 1: Create .env.development**

Write to [cms.frontend/.env.development](file:///D:/TrenLop/ASP.NET/AnhCMS_Solution/cms.frontend/.env.development):
```env
REACT_APP_API_URL=https://localhost:7224
REACT_APP_IMAGE_BASE_URL=https://localhost:7224
```

- [ ] **Step 2: Create .env.production**

Write to [cms.frontend/.env.production](file:///D:/TrenLop/ASP.NET/AnhCMS_Solution/cms.frontend/.env.production):
```env
REACT_APP_API_URL=https://api.tiemhoacuanam.com/api/v1
REACT_APP_IMAGE_BASE_URL=https://api.tiemhoacuanam.com
```

- [ ] **Step 3: Update .gitignore**

Modify [cms.frontend/.gitignore](file:///D:/TrenLop/ASP.NET/AnhCMS_Solution/cms.frontend/.gitignore) by appending the following lines:
```
# env files
.env
.env.development
.env.production
```

- [ ] **Step 4: Commit**

```bash
git add cms.frontend/.gitignore cms.frontend/.env.development cms.frontend/.env.production
git commit -m "feat: initialize env files and update gitignore for frontend"
```

---

### Task 2: Implement Central Config and Refactor API Utils

**Files:**
- Create: `cms.frontend/src/config/index.ts`
- Modify: `cms.frontend/src/utils/apiUtils.ts`

**Interfaces:**
- Produces: Type-safe centralized config mapping and updated endpoint helpers.

- [ ] **Step 1: Create config file**

Write to [cms.frontend/src/config/index.ts](file:///D:/TrenLop/ASP.NET/AnhCMS_Solution/cms.frontend/src/config/index.ts):
```typescript
const CONFIG = {
  API_BASE_URL: process.env.REACT_APP_API_URL || 'https://localhost:7224',
  IMAGE_BASE_URL: process.env.REACT_APP_IMAGE_BASE_URL || 'https://localhost:7224',
  TIMEOUT: 10000,
};

export default CONFIG;
```

- [ ] **Step 2: Refactor apiUtils.ts**

Overwrite [cms.frontend/src/utils/apiUtils.ts](file:///D:/TrenLop/ASP.NET/AnhCMS_Solution/cms.frontend/src/utils/apiUtils.ts) with the following content:
```typescript
import CONFIG from '../config';

export const API_BASE_URL: string = CONFIG.API_BASE_URL;

export const getImageUrl = (path?: string): string => {
  if (!path) return 'https://via.placeholder.com/400x400?text=No+Image';
  if (path.startsWith('http')) return path;
  return `${CONFIG.IMAGE_BASE_URL}${path.startsWith('/') ? '' : '/'}${path}`;
};
```

- [ ] **Step 3: Verify TypeScript Compilation**

Run: `npx tsc --noEmit` inside `cms.frontend/`
Expected: Compile successfully.

- [ ] **Step 4: Commit**

```bash
git add cms.frontend/src/config/index.ts cms.frontend/src/utils/apiUtils.ts
git commit -m "feat: add central CONFIG and update apiUtils to consume CONFIG values"
```
