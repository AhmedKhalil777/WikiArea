import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { 
  WikiPage, 
  WikiPageSummary, 
  WikiFolder, 
  WikiFolderTree, 
  Comment, 
  CreateWikiPageRequest, 
  UpdateWikiPageRequest, 
  SearchResult 
} from '../models/wiki.models';

@Injectable({
  providedIn: 'root'
})
export class WikiService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/api`;

  // Wiki Pages
  getWikiPageById(id: string): Observable<WikiPage> {
    return this.http.get<WikiPage>(`${this.apiUrl}/wikipages/${id}`);
  }

  getWikiPageBySlug(slug: string): Observable<WikiPage> {
    return this.http.get<WikiPage>(`${this.apiUrl}/wikipages/slug/${slug}`);
  }

  getWikiPagesByFolder(folderId: string): Observable<WikiPageSummary[]> {
    return this.http.get<WikiPageSummary[]>(`${this.apiUrl}/wikipages/folder/${folderId}`);
  }

  getRecentWikiPages(count: number = 10): Observable<WikiPageSummary[]> {
    const params = new HttpParams().set('count', count.toString());
    return this.http.get<WikiPageSummary[]>(`${this.apiUrl}/wikipages/recent`, { params });
  }

  createWikiPage(request: CreateWikiPageRequest): Observable<WikiPage> {
    return this.http.post<WikiPage>(`${this.apiUrl}/wikipages`, request);
  }

  updateWikiPage(request: UpdateWikiPageRequest): Observable<WikiPage> {
    return this.http.put<WikiPage>(`${this.apiUrl}/wikipages/${request.id}`, request);
  }

  deleteWikiPage(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/wikipages/${id}`);
  }

  publishWikiPage(id: string): Observable<WikiPage> {
    return this.http.post<WikiPage>(`${this.apiUrl}/wikipages/${id}/publish`, {});
  }

  submitForReview(id: string): Observable<WikiPage> {
    return this.http.post<WikiPage>(`${this.apiUrl}/wikipages/${id}/submit-review`, {});
  }

  approveReview(id: string, notes: string): Observable<WikiPage> {
    return this.http.post<WikiPage>(`${this.apiUrl}/wikipages/${id}/approve`, { notes });
  }

  rejectReview(id: string, notes: string): Observable<WikiPage> {
    return this.http.post<WikiPage>(`${this.apiUrl}/wikipages/${id}/reject`, { notes });
  }

  searchWikiPages(query: string, pageSize: number = 20, pageNumber: number = 1): Observable<SearchResult> {
    const params = new HttpParams()
      .set('q', query)
      .set('pageSize', pageSize.toString())
      .set('pageNumber', pageNumber.toString());
    return this.http.get<SearchResult>(`${this.apiUrl}/wikipages/search`, { params });
  }

  // Wiki Folders
  getRootFolders(): Observable<WikiFolderTree[]> {
    return this.http.get<WikiFolderTree[]>(`${this.apiUrl}/wikifolders/root`);
  }

  getWikiFolderById(id: string): Observable<WikiFolder> {
    return this.http.get<WikiFolder>(`${this.apiUrl}/wikifolders/${id}`);
  }

  createWikiFolder(name: string, description: string, parentFolderId?: string): Observable<WikiFolder> {
    const request = { name, description, parentFolderId };
    return this.http.post<WikiFolder>(`${this.apiUrl}/wikifolders`, request);
  }

  updateWikiFolder(id: string, name: string, description: string): Observable<WikiFolder> {
    const request = { name, description };
    return this.http.put<WikiFolder>(`${this.apiUrl}/wikifolders/${id}`, request);
  }

  deleteWikiFolder(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/wikifolders/${id}`);
  }

  // Comments
  getCommentsByPage(pageId: string): Observable<Comment[]> {
    return this.http.get<Comment[]>(`${this.apiUrl}/comments/page/${pageId}`);
  }

  getCommentById(id: string): Observable<Comment> {
    return this.http.get<Comment>(`${this.apiUrl}/comments/${id}`);
  }

  createComment(pageId: string, content: string, parentCommentId?: string): Observable<Comment> {
    const request = { wikiPageId: pageId, content, parentCommentId };
    return this.http.post<Comment>(`${this.apiUrl}/comments`, request);
  }

  updateComment(id: string, content: string): Observable<Comment> {
    const request = { content };
    return this.http.put<Comment>(`${this.apiUrl}/comments/${id}`, request);
  }

  resolveComment(id: string): Observable<Comment> {
    return this.http.post<Comment>(`${this.apiUrl}/comments/${id}/resolve`, {});
  }

  deleteComment(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/comments/${id}`);
  }
} 