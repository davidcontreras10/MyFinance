import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { GlobalVariables } from '../global-variables';
import { catchError, map } from 'rxjs/operators';
import { Observable, of } from 'rxjs';
import { BasicOption, ExecutedTask, IAutomaticTask, ScheduleTaskView, SpinnerController, UserSelectAccount } from '../automatic-tasks/automatic-tasks.model';
import { BasicNewScheduledTask, TransferNewScheduledTask } from './models';

@Injectable({
  providedIn: 'root'
})
export class MyFinanceService {

  private baseUrl!: string;

  constructor(globalVariables: GlobalVariables, private http: HttpClient, private spinnerController: SpinnerController) {
    this.baseUrl = globalVariables.baseUrl;
  }

  deleteScheduledTask(taskId: string) {
    const url = `${this.baseUrl}/DeleteScheduledTaskAsync`;
    const params = new HttpParams().set('taskId', taskId);
    this.spinnerController.enableSpinner();
    return this.http.delete(url, { params: params })
      .pipe(
        map(x => {
          this.spinnerController.disableSpinner();
          return x;
        }),
        catchError(this.handleError('delete', []))
      )
  }

  getScheduledTasks(): Observable<IAutomaticTask[]> {
    const url = `${this.baseUrl}/GetScheduledTasksAsync`;
    this.spinnerController.enableSpinner();
    return this.http.get<IAutomaticTask[]>(url)
      .pipe(
        map(x => {
          this.spinnerController.disableSpinner();
          return x;
        }),
        catchError(this.handleError<IAutomaticTask[]>('get scheduled tasks', []))
      )
  }

  getDestinationAccounts(accountPeriodId: number, currencyId: number): Observable<BasicOption[]> {
    const url = `${this.baseUrl}/GetPossibleDestinationAccountAsync`;
    const params = new HttpParams().set('accountPeriodId', accountPeriodId).set('currencyId', currencyId);
    this.spinnerController.enableSpinner();
    return this.http.get<BasicOption[]>(url, { params: params })
      .pipe(
        catchError(this.handleError<BasicOption[]>('get destination account', []))
      );
  }

  getUserAccounts(): Observable<UserSelectAccount[]> {
    const url = `${this.baseUrl}/GetUserAccountsAsync`;
    this.spinnerController.enableSpinner();
    return this.http.get<UserSelectAccount[]>(url)
      .pipe(
        map((responses: UserSelectAccount[]) => {
          this.spinnerController.disableSpinner();
          return responses;
        }),
        catchError(this.handleError<UserSelectAccount[]>('get user accounts', []))
      );
  }

  getAddSpendData(accountPeriodId: number) {
    const params = new HttpParams().set('accountPeriodId', accountPeriodId);
    const url = `${this.baseUrl}/GetAddSpendViewModelAsync`;
    return this.http.get(url, { params: params })
      .pipe(map((response: any) => response));
  }

  createBasic(model: BasicNewScheduledTask) {
    const url = `${this.baseUrl}/CreateBasicAsync`;
    return this.http.post(url, model);
  }

  createTransfer(model: TransferNewScheduledTask) {
    const url = `${this.baseUrl}/CreateTransferAsync`;
    return this.http.post(url, model);
  }

  getExecutedTasks(taskId: string): Observable<ExecutedTask[]> {
    const url = `${this.baseUrl}/GetExecutedTaskAsync`;
    const params = new HttpParams().set('taskId', taskId);
    return this.http.get(url, { params: params })
      .pipe(map((responses: any) => responses));;
  }

  private _mapScheduledTasks(tasks: IAutomaticTask[]): IAutomaticTask[] {
    return tasks;
  }

  /**
   * Handle Http operation that failed.
   * Let the app continue.
   *
   * @param operation - name of the operation that failed
   * @param result - optional value to return as the observable result
   */
  private handleError<T>(operation = 'unknown operation', result?: T) {
    return (error: any): Observable<T> => {
      this.spinnerController.disableSpinner();
      // TODO: send the error to remote logging infrastructure
      console.error(operation, error); // log to console instead
      // Let the app keep running by returning an empty result.
      return of(result as T);
    };
  }
}
