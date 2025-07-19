import { Component, Input, Output, EventEmitter, forwardRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatTabsModule } from '@angular/material/tabs';

@Component({
  selector: 'app-markdown-editor',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatButtonModule,
    MatIconModule,
    MatToolbarModule,
    MatTooltipModule,
    MatTabsModule
  ],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => MarkdownEditorComponent),
      multi: true
    }
  ],
  template: `
    <div class="markdown-editor">
      <!-- Toolbar -->
      <mat-toolbar class="editor-toolbar">
        <button mat-icon-button (click)="insertMarkdown('**', '**')" matTooltip="Bold">
          <mat-icon>format_bold</mat-icon>
        </button>
        <button mat-icon-button (click)="insertMarkdown('*', '*')" matTooltip="Italic">
          <mat-icon>format_italic</mat-icon>
        </button>
        <button mat-icon-button (click)="insertMarkdown('~~', '~~')" matTooltip="Strikethrough">
          <mat-icon>format_strikethrough</mat-icon>
        </button>
        <button mat-icon-button (click)="insertMarkdown('# ', '')" matTooltip="Header 1">
          <mat-icon>title</mat-icon>
        </button>
        <button mat-icon-button (click)="insertMarkdown('- ', '')" matTooltip="Bullet List">
          <mat-icon>format_list_bulleted</mat-icon>
        </button>
        <button mat-icon-button (click)="insertMarkdown('1. ', '')" matTooltip="Numbered List">
          <mat-icon>format_list_numbered</mat-icon>
        </button>
        <button mat-icon-button (click)="insertMarkdown('[', '](url)')" matTooltip="Link">
          <mat-icon>link</mat-icon>
        </button>
        <button mat-icon-button (click)="insertMarkdown('![alt](', ')')" matTooltip="Image">
          <mat-icon>image</mat-icon>
        </button>
        <button mat-icon-button (click)="insertCodeBlock()" matTooltip="Code Block">
          <mat-icon>code</mat-icon>
        </button>
        <button mat-icon-button (click)="insertMarkdown('> ', '')" matTooltip="Quote">
          <mat-icon>format_quote</mat-icon>
        </button>
        <button mat-icon-button (click)="insertTable()" matTooltip="Table">
          <mat-icon>table_chart</mat-icon>
        </button>
      </mat-toolbar>

      <!-- Editor Content -->
      <div class="editor-content">
        <mat-tab-group [(selectedIndex)]="selectedTab">
          <mat-tab label="Edit">
            <div class="edit-area">
              <textarea
                #textArea
                [(ngModel)]="content"
                (input)="onContentChange($event)"
                (keydown)="onKeyDown($event)"
                placeholder="Enter your markdown content here..."
                class="markdown-textarea">
              </textarea>
            </div>
          </mat-tab>
          <mat-tab label="Preview">
            <div class="preview-area">
              <div class="markdown-preview" [innerHTML]="previewHtml"></div>
            </div>
          </mat-tab>
          <mat-tab label="Split View">
            <div class="split-view">
              <div class="editor-pane">
                <textarea
                  #splitTextArea
                  [(ngModel)]="content"
                  (input)="onContentChange($event)"
                  (keydown)="onKeyDown($event)"
                  placeholder="Enter your markdown content here..."
                  class="markdown-textarea split">
                </textarea>
              </div>
              <div class="preview-pane">
                <div class="markdown-preview" [innerHTML]="previewHtml"></div>
              </div>
            </div>
          </mat-tab>
        </mat-tab-group>
      </div>
    </div>
  `,
  styles: [`
    .markdown-editor {
      border: 1px solid #ddd;
      border-radius: 4px;
      overflow: hidden;
    }

    .editor-toolbar {
      background: #f5f5f5;
      min-height: 48px;
      padding: 0 8px;
    }

    .editor-content {
      min-height: 400px;
    }

    .edit-area, .preview-area {
      height: 400px;
      overflow: auto;
    }

    .split-view {
      display: flex;
      height: 400px;
    }

    .editor-pane, .preview-pane {
      flex: 1;
      height: 100%;
      overflow: auto;
    }

    .editor-pane {
      border-right: 1px solid #ddd;
    }

    .markdown-textarea {
      width: 100%;
      height: 100%;
      border: none;
      outline: none;
      padding: 16px;
      font-family: 'Courier New', monospace;
      font-size: 14px;
      line-height: 1.5;
      resize: none;
      background: #fff;
    }

    .markdown-textarea.split {
      border-right: 1px solid #ddd;
    }

    .markdown-preview {
      padding: 16px;
      line-height: 1.6;
      font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif;
    }

    .markdown-preview h1, .markdown-preview h2, .markdown-preview h3,
    .markdown-preview h4, .markdown-preview h5, .markdown-preview h6 {
      margin-top: 24px;
      margin-bottom: 16px;
      font-weight: 600;
      line-height: 1.25;
    }

    .markdown-preview h1 {
      font-size: 2em;
      border-bottom: 1px solid #eaecef;
      padding-bottom: 0.3em;
    }

    .markdown-preview h2 {
      font-size: 1.5em;
      border-bottom: 1px solid #eaecef;
      padding-bottom: 0.3em;
    }

    .markdown-preview h3 {
      font-size: 1.25em;
    }

    .markdown-preview p {
      margin-bottom: 16px;
    }

    .markdown-preview ul, .markdown-preview ol {
      margin-bottom: 16px;
      padding-left: 2em;
    }

    .markdown-preview li {
      margin-bottom: 4px;
    }

    .markdown-preview blockquote {
      margin: 0 0 16px 0;
      padding: 0 1em;
      color: #6a737d;
      border-left: 0.25em solid #dfe2e5;
    }

    .markdown-preview pre {
      background: #f6f8fa;
      border-radius: 6px;
      font-size: 85%;
      line-height: 1.45;
      overflow: auto;
      padding: 16px;
      margin-bottom: 16px;
    }

    .markdown-preview code {
      background: #f6f8fa;
      border-radius: 3px;
      font-size: 85%;
      padding: 0.2em 0.4em;
    }

    .markdown-preview table {
      border-collapse: collapse;
      border-spacing: 0;
      width: 100%;
      margin-bottom: 16px;
    }

    .markdown-preview table th,
    .markdown-preview table td {
      border: 1px solid #dfe2e5;
      padding: 6px 13px;
    }

    .markdown-preview table th {
      background: #f6f8fa;
      font-weight: 600;
    }

    ::ng-deep .mat-mdc-tab-body-wrapper {
      height: 100%;
    }

    ::ng-deep .mat-mdc-tab-body-content {
      height: 100%;
      overflow: hidden;
    }
  `]
})
export class MarkdownEditorComponent implements ControlValueAccessor {
  @Input() placeholder = 'Enter your markdown content here...';
  @Output() contentChange = new EventEmitter<string>();

  content = '';
  previewHtml = '';
  selectedTab = 0;

  private onChange = (value: string) => {};
  private onTouched = () => {};

  onContentChange(event: any) {
    this.content = event.target.value;
    this.updatePreview();
    this.onChange(this.content);
    this.contentChange.emit(this.content);
  }

  onKeyDown(event: KeyboardEvent) {
    if (event.key === 'Tab') {
      event.preventDefault();
      this.insertText('  '); // Insert 2 spaces for tab
    }
  }

  insertMarkdown(before: string, after: string) {
    const textarea = document.querySelector('.markdown-textarea') as HTMLTextAreaElement;
    if (!textarea) return;

    const start = textarea.selectionStart;
    const end = textarea.selectionEnd;
    const selectedText = this.content.substring(start, end);
    
    const newText = before + selectedText + after;
    const newContent = this.content.substring(0, start) + newText + this.content.substring(end);
    
    this.content = newContent;
    this.updatePreview();
    this.onChange(this.content);
    this.contentChange.emit(this.content);

    // Set cursor position
    setTimeout(() => {
      if (selectedText) {
        textarea.setSelectionRange(start + before.length, start + before.length + selectedText.length);
      } else {
        textarea.setSelectionRange(start + before.length, start + before.length);
      }
      textarea.focus();
    }, 10);
  }

  insertCodeBlock() {
    this.insertMarkdown('\n```\n', '\n```\n');
  }

  insertTable() {
    const table = '\n| Header 1 | Header 2 | Header 3 |\n|----------|----------|----------|\n| Cell 1   | Cell 2   | Cell 3   |\n| Cell 4   | Cell 5   | Cell 6   |\n';
    this.insertText(table);
  }

  insertText(text: string) {
    const textarea = document.querySelector('.markdown-textarea') as HTMLTextAreaElement;
    if (!textarea) return;

    const start = textarea.selectionStart;
    const end = textarea.selectionEnd;
    
    const newContent = this.content.substring(0, start) + text + this.content.substring(end);
    
    this.content = newContent;
    this.updatePreview();
    this.onChange(this.content);
    this.contentChange.emit(this.content);

    setTimeout(() => {
      textarea.setSelectionRange(start + text.length, start + text.length);
      textarea.focus();
    }, 10);
  }

  updatePreview() {
    // Simple markdown to HTML conversion
    // In a real app, you'd use a library like marked or markdown-it
    let html = this.content
      // Headers
      .replace(/^# (.*$)/gim, '<h1>$1</h1>')
      .replace(/^## (.*$)/gim, '<h2>$1</h2>')
      .replace(/^### (.*$)/gim, '<h3>$1</h3>')
      .replace(/^#### (.*$)/gim, '<h4>$1</h4>')
      .replace(/^##### (.*$)/gim, '<h5>$1</h5>')
      .replace(/^###### (.*$)/gim, '<h6>$1</h6>')
      // Bold
      .replace(/\*\*(.*?)\*\*/g, '<strong>$1</strong>')
      // Italic
      .replace(/\*(.*?)\*/g, '<em>$1</em>')
      // Strikethrough
      .replace(/~~(.*?)~~/g, '<del>$1</del>')
      // Code blocks
      .replace(/```([\s\S]*?)```/g, '<pre><code>$1</code></pre>')
      // Inline code
      .replace(/`(.*?)`/g, '<code>$1</code>')
      // Links
      .replace(/\[([^\]]+)\]\(([^)]+)\)/g, '<a href="$2" target="_blank">$1</a>')
      // Images
      .replace(/!\[([^\]]*)\]\(([^)]+)\)/g, '<img src="$2" alt="$1" style="max-width: 100%;">')
      // Quotes
      .replace(/^> (.*$)/gim, '<blockquote>$1</blockquote>')
      // Unordered lists
      .replace(/^\- (.*$)/gim, '<li>$1</li>')
      // Ordered lists
      .replace(/^\d+\. (.*$)/gim, '<li>$1</li>')
      // Line breaks
      .replace(/\n/g, '<br>');

    // Wrap lists
    html = html.replace(/(<li>.*<\/li>)/gs, '<ul>$1</ul>');
    html = html.replace(/(<blockquote>.*<\/blockquote>)/gs, '<div class="blockquote-container">$1</div>');

    this.previewHtml = html;
  }

  // ControlValueAccessor implementation
  writeValue(value: string): void {
    this.content = value || '';
    this.updatePreview();
  }

  registerOnChange(fn: (value: string) => void): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: () => void): void {
    this.onTouched = fn;
  }

  setDisabledState(isDisabled: boolean): void {
    // Handle disabled state if needed
  }
} 