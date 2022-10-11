import { Component, Input, OnInit } from '@angular/core';
import { IAutomaticTask } from '../automatic-tasks/automatic-tasks.model';

@Component({
  selector: 'app-task-detail',
  templateUrl: './task-detail.component.html',
  styleUrls: ['./task-detail.component.css']
})
export class TaskDetailComponent implements OnInit {

  @Input()
  selectedTask!: IAutomaticTask

  ngOnInit(): void {
  }

}
