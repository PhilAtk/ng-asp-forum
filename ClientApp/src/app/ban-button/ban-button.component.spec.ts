import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BanButtonComponent } from './ban-button.component';

describe('BanButtonComponent', () => {
  let component: BanButtonComponent;
  let fixture: ComponentFixture<BanButtonComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [BanButtonComponent]
    });
    fixture = TestBed.createComponent(BanButtonComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
