import { Component, OnInit } from '@angular/core';

export interface AutomaticTask {
  Id: number,
  Name: string
}

@Component({
  selector: 'app-automatic-tasks',
  templateUrl: './automatic-tasks.component.html',
  styleUrls: ['./automatic-tasks.component.css']
})
export class AutomaticTasksComponent {
  public selectedTask: AutomaticTask;

  constructor() {
    this.selectedTask = { Id: 0, Name: "" };
  }

  public onSelectedTaskChanged(selectedOption: AutomaticTask): void {
    console.log('Task changed received: ', selectedOption);
    this.selectedTask = selectedOption;
  }

  loadedTasks: AutomaticTask[] = [
    {
      Id: 1,
      Name: "100$ every month"
    },
    {
      Id: 2,
      Name: "50$ every week"
    },
    {
      Id: 3,
      Name: "300$ every month"
    },
    {
      Id: 4,
      Name: "100$ every month"
    },
    {
      Id: 5,
      Name: "50$ every week"
    },
    {
      Id: 6,
      Name: "300$ every month"
    },
    {
      Id: 7,
      Name: "100$ every month"
    },
    {
      Id: 8,
      Name: "50$ every week"
    },
    {
      Id: 9,
      Name: "300$ every month"
    }
  ];

}
