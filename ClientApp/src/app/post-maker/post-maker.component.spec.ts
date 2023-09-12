import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PostMakerComponent } from './post-maker.component';

describe('PostMakerComponent', () => {
  let component: PostMakerComponent;
  let fixture: ComponentFixture<PostMakerComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ PostMakerComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PostMakerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
