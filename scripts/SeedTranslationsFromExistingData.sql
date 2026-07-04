-- Seed existing single-language data into 'en' translations
-- Run this BEFORE applying the AddMultiLanguageSupport migration
-- so existing content is preserved in the new translation tables.

INSERT INTO ProductTranslations (ProductId, Locale, Name, Description, Slug)
SELECT Id, 'en', Name, Description, '' FROM Products
WHERE Name IS NOT NULL AND Name != '';

INSERT INTO CategoryProductTranslations (CategoryProductId, Locale, Name, Description, Slug)
SELECT Id, 'en', Name, Description, '' FROM CategoriesProducts
WHERE Name IS NOT NULL AND Name != '';

INSERT INTO CategoryTranslations (CategoryId, Locale, Name, Description, Slug)
SELECT Id, 'en', Name, Description, '' FROM Categories
WHERE Name IS NOT NULL AND Name != '';

INSERT INTO PostTranslations (PostId, Locale, Title, Content, Summary, Slug)
SELECT Id, 'en', Title, Content, Summary, '' FROM Posts
WHERE Title IS NOT NULL AND Title != '';
