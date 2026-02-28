import { provideHttpClient } from '@angular/common/http';
import { TestBed } from '@angular/core/testing';
import { RouterModule } from '@angular/router';

import Keycloak from 'keycloak-js';
import { beforeEach, describe, expect, it, vi } from 'vitest';

import { App } from './app';

describe('App', () => {
  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [App, RouterModule.forRoot([])],
      providers: [
        provideHttpClient(),
        {
          provide: Keycloak,
          useValue: {
            init: vi.fn().mockResolvedValue(true),
            isLoggedIn: vi.fn().mockReturnValue(false),
            getToken: vi.fn().mockReturnValue(''),
            login: vi.fn(),
            register: vi.fn(),
            logout: vi.fn(),
          },
        },
      ],
    }).compileComponents();
  });

  it('should create the app', () => {
    const fixture = TestBed.createComponent(App);
    const app = fixture.componentInstance;
    expect(app).toBeTruthy();
  });
});
