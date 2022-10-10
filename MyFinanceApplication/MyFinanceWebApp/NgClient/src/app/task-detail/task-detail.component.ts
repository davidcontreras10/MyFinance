import { Component, Input, OnInit } from '@angular/core';
import { AutomaticTask } from '../automatic-tasks/automatic-tasks.component';

@Component({
  selector: 'app-task-detail',
  templateUrl: './task-detail.component.html',
  styleUrls: ['./task-detail.component.css']
})
export class TaskDetailComponent implements OnInit {

  @Input()
  selectedTask: AutomaticTask

  constructor() { 
    this.selectedTask = {
      Id: 0,
      Name: ""
    }
  }

  ngOnInit(): void {
  }

}
