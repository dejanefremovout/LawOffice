# Office Service Usage Guide

The `OfficeService` manages a global office ID that is accessible from all pages and persisted in a cookie.

## Features

- **Global State**: The office ID is available across all components
- **Persistent**: Stored in a cookie that expires after 365 days
- **Reactive**: Uses Angular signals for automatic UI updates
- **Type-safe**: Full TypeScript support
- **SSR-compatible**: Safely handles server-side rendering

## Usage

### 1. Import and Inject the Service

```typescript
import { Component, inject } from '@angular/core';
import { OfficeService } from '../../services/office.service';

@Component({
  selector: 'app-my-component',
  templateUrl: './my-component.html',
})
export class MyComponent {
  // Inject the office service
  protected readonly officeService = inject(OfficeService);
  
  // Access signals directly
  protected readonly officeId = this.officeService.officeId;
  protected readonly hasOfficeId = this.officeService.hasOfficeId;
}
```

### 2. Read the Office ID

#### In Component TypeScript

```typescript
export class MyComponent {
  protected readonly officeService = inject(OfficeService);
  
  loadData(): void {
    const currentOfficeId = this.officeService.officeId();
    
    if (this.officeService.hasOfficeId()) {
      console.log('Office ID:', currentOfficeId);
      // Use the office ID to fetch data
    }
  }
}
```

#### In Component Template

```html
@if (officeService.hasOfficeId()) {
  <p>Current Office: {{ officeService.officeId() }}</p>
} @else {
  <p>No office selected</p>
}
```

### 3. Set the Office ID

```typescript
export class MyComponent {
  protected readonly officeService = inject(OfficeService);
  
  selectOffice(officeId: string): void {
    // This will update the signal and save to cookie automatically
    this.officeService.setOfficeId(officeId);
  }
}
```

### 4. Clear the Office ID

```typescript
export class MyComponent {
  protected readonly officeService = inject(OfficeService);
  
  logout(): void {
    // This will clear the signal and delete the cookie
    this.officeService.clearOfficeId();
  }
}
```

### 5. Use with Computed Signals

```typescript
import { computed } from '@angular/core';

export class MyComponent {
  protected readonly officeService = inject(OfficeService);
  
  // Create derived state
  protected readonly welcomeMessage = computed(() => {
    const officeId = this.officeService.officeId();
    return officeId 
      ? `Welcome to Office ${officeId}` 
      : 'Please select an office';
  });
}
```

### 6. Use with Effects

```typescript
import { effect } from '@angular/core';

export class MyComponent {
  protected readonly officeService = inject(OfficeService);
  
  constructor() {
    // React to office ID changes
    effect(() => {
      const officeId = this.officeService.officeId();
      if (officeId) {
        console.log('Office changed to:', officeId);
        // Fetch office-specific data
      }
    });
  }
}
```

## API Reference

### Properties

- `officeId: Signal<string | null>` - Readonly signal containing the current office ID
- `hasOfficeId: Signal<boolean>` - Computed signal indicating if an office ID is set

### Methods

- `setOfficeId(id: string): void` - Sets the office ID (updates signal and cookie)
- `clearOfficeId(): void` - Clears the office ID (removes signal value and cookie)

## Cookie Details

- **Name**: `officeId`
- **Expiration**: 365 days
- **Path**: `/` (available site-wide)
- **SameSite**: `Strict` (security measure)
- **Encoding**: URI encoded for special characters

## Example: Office Selector Component

```typescript
import { Component, inject } from '@angular/core';
import { OfficeService } from '../../services/office.service';

@Component({
  selector: 'app-office-selector',
  template: `
    <div class="office-selector">
      <h3>Select Office</h3>
      @if (officeService.hasOfficeId()) {
        <p>Current: {{ officeService.officeId() }}</p>
        <button (click)="clearOffice()">Clear Selection</button>
      } @else {
        <p>No office selected</p>
      }
      
      <div class="office-list">
        @for (office of offices; track office.id) {
          <button (click)="selectOffice(office.id)">
            {{ office.name }}
          </button>
        }
      </div>
    </div>
  `
})
export class OfficeSelectorComponent {
  protected readonly officeService = inject(OfficeService);
  
  protected readonly offices = [
    { id: 'office-001', name: 'Main Office' },
    { id: 'office-002', name: 'Branch Office' },
    { id: 'office-003', name: 'Regional Office' }
  ];
  
  selectOffice(officeId: string): void {
    this.officeService.setOfficeId(officeId);
  }
  
  clearOffice(): void {
    this.officeService.clearOfficeId();
  }
}
```
