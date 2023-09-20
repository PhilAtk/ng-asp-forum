import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PostAuditPageComponent } from './post-audit-page.component';

describe('PostAuditPageComponent', () => {
  let component: PostAuditPageComponent;
  let fixture: ComponentFixture<PostAuditPageComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [PostAuditPageComponent]
    });
    fixture = TestBed.createComponent(PostAuditPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
