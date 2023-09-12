import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ThreadMakerComponent } from './thread-maker.component';

describe('ThreadMakerComponent', () => {
  let component: ThreadMakerComponent;
  let fixture: ComponentFixture<ThreadMakerComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ThreadMakerComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ThreadMakerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
