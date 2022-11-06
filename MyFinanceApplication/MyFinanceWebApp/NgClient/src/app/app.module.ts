import {HttpClientModule} from '@angular/common/http'
import {HttpClient} from '@angular/common/http'
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { AutomaticTasksComponent } from './automatic-tasks/automatic-tasks.component';
import { BrowserAnimationsModule, NoopAnimationsModule } from '@angular/platform-browser/animations';
import { MatTableModule } from '@angular/material/table'
import {MatListModule} from '@angular/material/list';
import { TasksListComponent } from './tasks-list/tasks-list.component';
import { TaskDetailComponent } from './task-detail/task-detail.component';
import { ExecutedTasksComponent } from './executed-tasks/executed-tasks.component';
import { TaskStatusComponent } from './task-status/task-status.component';
import { NewScheduledTaskComponent } from './new-scheduled-task/new-scheduled-task.component';
import { FormsModule }   from '@angular/forms';
import { GlobalVariables } from './global-variables';
import { MyFinanceService } from './services/my-finance.service';
import { ProgressSpinnerComponent, ProgressSpinnerModule } from './progress-spinner/progress-spinner.module';
import { AppOverlayModule } from './overlay/overlay.module';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';


@NgModule({
  declarations: [
    AppComponent,
    AutomaticTasksComponent,
    TasksListComponent,
    TaskDetailComponent,
    ExecutedTasksComponent,
    TaskStatusComponent,
    NewScheduledTaskComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    NoopAnimationsModule,
    MatTableModule,
    MatListModule,
    FormsModule,
    HttpClientModule,
    AppOverlayModule,
    ProgressSpinnerModule,
    MatProgressSpinnerModule,
    BrowserAnimationsModule
  ],
  providers: [
    HttpClient,
    GlobalVariables,
    MyFinanceService
  ],
  entryComponents: [AppComponent,ProgressSpinnerComponent],
  bootstrap: [AppComponent]
})
export class AppModule { }
