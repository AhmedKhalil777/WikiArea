export interface WikiPage {
  id: string;
  title: string;
  slug: string;
  content: string;
  contentType: string;
  folderId?: string;
  status: PageStatus;
  version: number;
  tags: string[];
  attachments: string[];
  isPublic: boolean;
  allowedRoles: string[];
  reviewerId?: string;
  reviewedAt?: Date;
  reviewNotes: string;
  viewCount: number;
  likeCount: number;
  metadata: PageMetadata;
  createdAt: Date;
  updatedAt: Date;
  createdBy: string;
  updatedBy: string;
  author?: User;
  reviewer?: User;
  folder?: WikiFolder;
}

export interface WikiPageSummary {
  id: string;
  title: string;
  slug: string;
  status: string;
  version: number;
  viewCount: number;
  likeCount: number;
  updatedAt: Date;
  updatedBy: string;
  folderId?: string;
  tags: string[];
}

export interface WikiFolder {
  id: string;
  name: string;
  description: string;
  path: string;
  parentFolderId?: string;
  sortOrder: number;
  tags: string[];
  isPublic: boolean;
  allowedRoles: string[];
  createdAt: Date;
  updatedAt: Date;
  createdBy: string;
  updatedBy: string;
  parentFolder?: WikiFolder;
  subFolders: WikiFolder[];
  pages: WikiPageSummary[];
}

export interface WikiFolderTree {
  id: string;
  name: string;
  path: string;
  parentFolderId?: string;
  sortOrder: number;
  isPublic: boolean;
  children: WikiFolderTree[];
  pageCount: number;
}

export interface User {
  id: string;
  username: string;
  email: string;
  displayName: string;
  adfsId: string;
  role: string;
  status: string;
  department: string;
  avatarUrl: string;
  lastLoginAt: Date;
  permissions: string[];
  createdAt: Date;
  updatedAt: Date;
}

export interface UserSummary {
  id: string;
  username: string;
  displayName: string;
  role: string;
  department: string;
  avatarUrl: string;
}

export interface Comment {
  id: string;
  wikiPageId: string;
  authorId: string;
  content: string;
  parentCommentId?: string;
  isResolved: boolean;
  resolvedBy?: string;
  resolvedAt?: Date;
  mentions: string[];
  likeCount: number;
  createdAt: Date;
  updatedAt: Date;
  author?: UserSummary;
  resolvedByUser?: UserSummary;
  replies: Comment[];
}

export interface PageMetadata {
  metaTitle: string;
  metaDescription: string;
  keywords: string[];
  author: string;
  language: string;
  isIndexable: boolean;
  customProperties: Record<string, string>;
}

export type PageStatus = 'Draft' | 'UnderReview' | 'Published' | 'Archived';
export type ContentType = 'Markdown' | 'Html' | 'PlainText' | 'Video' | 'Image' | 'Document';
export type UserRole = 'Reader' | 'Writer' | 'Reviewer' | 'Administrator';

export interface CreateWikiPageRequest {
  title: string;
  content: string;
  contentType: string;
  folderId?: string;
  isPublic: boolean;
  tags: string[];
  allowedRoles: string[];
}

export interface UpdateWikiPageRequest {
  id: string;
  title: string;
  content: string;
  folderId?: string;
  isPublic: boolean;
  tags: string[];
  allowedRoles: string[];
}

export interface SearchResult {
  pages: WikiPageSummary[];
  totalCount: number;
  query: string;
} 