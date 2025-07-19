import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatListModule } from '@angular/material/list';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatDividerModule } from '@angular/material/divider';
import { RouterModule } from '@angular/router';
import { WikiService } from '../../core/services/wiki.service';
import { WikiPageSummary, SearchResult } from '../../core/models/wiki.models';
import { Subject, debounceTime, distinctUntilChanged, switchMap } from 'rxjs';

@Component({
  selector: 'app-search',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    RouterModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatListModule,
    MatProgressSpinnerModule,
    MatDividerModule
  ],
  templateUrl: './search.component.html',
  styleUrls: ['./search.component.scss']
})
export class SearchComponent implements OnInit {
  private wikiService = inject(WikiService);
  private searchSubject = new Subject<string>();
  
  searchQuery = '';
  searchResults: WikiPageSummary[] = [];
  isLoading = false;
  totalCount = 0;
  hasSearched = false;

  ngOnInit() {
    this.searchSubject
      .pipe(
        debounceTime(300),
        distinctUntilChanged(),
        switchMap(query => {
          if (query.trim().length < 2) {
            return [];
          }
          this.isLoading = true;
          return this.wikiService.searchWikiPages(query);
        })
      )
      .subscribe({
        next: (result: SearchResult) => {
          this.searchResults = result.pages;
          this.totalCount = result.totalCount;
          this.isLoading = false;
          this.hasSearched = true;
        },
        error: (error) => {
          console.error('Search error:', error);
          this.isLoading = false;
          this.hasSearched = true;
        }
      });
  }

  onSearchInput(query: string) {
    this.searchQuery = query;
    this.searchSubject.next(query);
  }

  onSearchInputEvent(event: Event) {
    const target = event.target as HTMLInputElement;
    this.onSearchInput(target.value);
  }

  search() {
    if (this.searchQuery.trim()) {
      this.searchSubject.next(this.searchQuery);
    }
  }

  clearSearch() {
    this.searchQuery = '';
    this.searchResults = [];
    this.hasSearched = false;
    this.totalCount = 0;
  }
} 