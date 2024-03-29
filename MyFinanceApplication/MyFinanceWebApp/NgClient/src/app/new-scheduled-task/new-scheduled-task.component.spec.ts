import { ComponentFixture, TestBed } from '@angular/core/testing';

import { NewScheduledTaskComponent } from './new-scheduled-task.component';

describe('NewScheduledTaskComponent', () => {
  let component: NewScheduledTaskComponent;
  let fixture: ComponentFixture<NewScheduledTaskComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ NewScheduledTaskComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(NewScheduledTaskComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
