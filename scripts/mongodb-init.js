// MongoDB initialization script for WikiArea
db = db.getSiblingDB('wikiarea');

// Create collections
db.createCollection('users');
db.createCollection('wiki_pages');
db.createCollection('wiki_folders');
db.createCollection('comments');

// Create initial indexes
db.users.createIndex({ "username": 1 }, { unique: true });
db.users.createIndex({ "email": 1 }, { unique: true });
db.users.createIndex({ "adfsId": 1 }, { unique: true });

db.wiki_pages.createIndex({ "slug": 1 }, { unique: true });
db.wiki_pages.createIndex({ "folderId": 1 });
db.wiki_pages.createIndex({ "status": 1 });
db.wiki_pages.createIndex({ "tags": 1 });
db.wiki_pages.createIndex({ "title": "text", "content": "text" });

db.wiki_folders.createIndex({ "path": 1 }, { unique: true });
db.wiki_folders.createIndex({ "parentFolderId": 1 });

db.comments.createIndex({ "wikiPageId": 1 });
db.comments.createIndex({ "authorId": 1 });

print('WikiArea database initialized successfully!'); 