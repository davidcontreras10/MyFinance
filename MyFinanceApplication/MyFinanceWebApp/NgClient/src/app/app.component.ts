import { Component, ElementRef } from '@angular/core';
import { ThemePalette } from '@angular/material/core';
import { ProgressSpinnerMode } from '@angular/material/progress-spinner';
import { ScheduleTaskRequestType, ScheduleTaskView, SpinnerController } from './automatic-tasks/automatic-tasks.model';
import { GlobalVariables } from './global-variables';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  ScheduleTaskRequestType = ScheduleTaskRequestType;
  spinnerColor?: ThemePalette = 'primary';
  spinnerMode: ProgressSpinnerMode = 'indeterminate';
  spinnerValue?: number;

  constructor(private elementRef: ElementRef,
    private globalVariables: GlobalVariables,
    public scheduleTaskView: ScheduleTaskView,
    public spinnerController: SpinnerController
    ) {
    this.globalVariables.baseUrl = this.elementRef.nativeElement.getAttribute('base-url');
    const reqUrlId = parseInt(this.elementRef.nativeElement.getAttribute('req-url'));

    const reqView = ScheduleTaskRequestType[ScheduleTaskRequestType[reqUrlId] as keyof typeof ScheduleTaskRequestType];
    scheduleTaskView.activeView = reqView;
  }
  title = 'myapp';
}
