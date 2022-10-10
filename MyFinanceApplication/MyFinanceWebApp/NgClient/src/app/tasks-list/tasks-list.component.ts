import { Component, Input, Output, OnInit, EventEmitter } from '@angular/core';
import { AutomaticTask } from '../automatic-tasks/automatic-tasks.component';

@Component({
  selector: 'app-tasks-list',
  templateUrl: './tasks-list.component.html',
  styleUrls: ['./tasks-list.component.css']
})
export class TasksListComponent {
  @Input() tasks: AutomaticTask[];
  @Output() selectedChanged = new EventEmitter();

  constructor(){
    this.tasks = []
  }

  public onSelection(selectedOption: AutomaticTask){
    this.selectedChanged.emit(selectedOption);
  }
}
