import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UnbanButtonComponent } from './unban-button.component';

describe('BanButtonComponent', () => {
  let component: UnbanButtonComponent;
  let fixture: ComponentFixture<UnbanButtonComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [UnbanButtonComponent]
    });
    fixture = TestBed.createComponent(UnbanButtonComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
