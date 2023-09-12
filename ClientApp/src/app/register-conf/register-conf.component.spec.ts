import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RegisterConfComponent } from './register-conf.component';

describe('RegisterConfComponent', () => {
  let component: RegisterConfComponent;
  let fixture: ComponentFixture<RegisterConfComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ RegisterConfComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(RegisterConfComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
