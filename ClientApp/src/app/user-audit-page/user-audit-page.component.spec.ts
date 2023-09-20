import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UserAuditPageComponent } from './user-audit-page.component';

describe('UserAuditPageComponent', () => {
  let component: UserAuditPageComponent;
  let fixture: ComponentFixture<UserAuditPageComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [UserAuditPageComponent]
    });
    fixture = TestBed.createComponent(UserAuditPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
