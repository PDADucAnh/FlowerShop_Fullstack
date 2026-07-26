# Task 7: LatestBlog — stacked vertical layout

**Status:** Done

**Commits:**
- `ca90995` - feat: change LatestBlog to stacked vertical cards

**Test summary:** TypeScript compiles cleanly (`npx tsc --noEmit` — no errors).

**Changes:**
- Replaced grid of `PostCard` components with vertically stacked `<Link>` cards
- Added `getImageUrl` import, `react-router-dom` `Link` usage
- Inlined card markup with image, category label, title, and description
- Updated layout classes to stacked vertical design with `space-y-stack-md`
