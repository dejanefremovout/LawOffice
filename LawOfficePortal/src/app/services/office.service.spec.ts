import { TestBed } from '@angular/core/testing';
import { OfficeService } from './office.service';
import { describe, it, expect, beforeEach, vi } from 'vitest';

describe('OfficeService', () => {
  let service: OfficeService;

  beforeEach(() => {
    // Clear all cookies before each test
    document.cookie.split(';').forEach((cookie) => {
      const eqPos = cookie.indexOf('=');
      const name = eqPos > -1 ? cookie.substring(0, eqPos) : cookie;
      document.cookie = name + '=;expires=Thu, 01 Jan 1970 00:00:00 GMT;path=/';
    });

    TestBed.configureTestingModule({});
    service = TestBed.inject(OfficeService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should initialize with null office ID when no cookie exists', () => {
    expect(service.officeId()).toBeNull();
    expect(service.hasOfficeId()).toBe(false);
  });

  it('should set and retrieve office ID', () => {
    const testId = 'office-123';
    service.setOfficeId(testId);
    expect(service.officeId()).toBe(testId);
    expect(service.hasOfficeId()).toBe(true);
  });

  it('should clear office ID', () => {
    service.setOfficeId('office-123');
    expect(service.officeId()).toBe('office-123');
    
    service.clearOfficeId();
    expect(service.officeId()).toBeNull();
    expect(service.hasOfficeId()).toBe(false);
  });

  it('should persist office ID in cookie', () => {
    return new Promise<void>((resolve) => {
      const testId = 'office-456';
      service.setOfficeId(testId);
      
      // Give effect time to run
      setTimeout(() => {
        const cookies = document.cookie;
        expect(cookies).toContain('officeId=' + encodeURIComponent(testId));
        resolve();
      }, 100);
    });
  });

  it('should load office ID from existing cookie', () => {
    // Set a cookie directly
    const testId = 'office-789';
    document.cookie = `officeId=${encodeURIComponent(testId)};path=/`;
    
    // Create a new TestBed to get a fresh service instance
    TestBed.resetTestingModule();
    TestBed.configureTestingModule({});
    const newService = TestBed.inject(OfficeService);
    expect(newService.officeId()).toBe(testId);
  });

  it('should delete cookie when clearing office ID', () => {
    return new Promise<void>((resolve) => {
      service.setOfficeId('office-999');
      
      setTimeout(() => {
        service.clearOfficeId();
        
        setTimeout(() => {
          const cookies = document.cookie;
          expect(cookies).not.toContain('officeId=');
          resolve();
        }, 100);
      }, 100);
    });
  });
});
