import { Component, ElementRef } from '@angular/core';
import { ScheduleTaskRequestType, ScheduleTaskView } from './automatic-tasks/automatic-tasks.model';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  ScheduleTaskRequestType = ScheduleTaskRequestType;

  public scheduleTaskView!: ScheduleTaskView;

  constructor(private elementRef: ElementRef) {
    const reqUrlId = parseInt(this.elementRef.nativeElement.getAttribute('req-url'));
    this.scheduleTaskView = {
      activeView: ScheduleTaskRequestType[ScheduleTaskRequestType[reqUrlId] as keyof typeof ScheduleTaskRequestType]
    }
  }
  title = 'myapp';
}
