import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ThreadAuditPageComponent } from './thread-audit-page.component';

describe('ThreadAuditPageComponent', () => {
  let component: ThreadAuditPageComponent;
  let fixture: ComponentFixture<ThreadAuditPageComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [ThreadAuditPageComponent]
    });
    fixture = TestBed.createComponent(ThreadAuditPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
