import { TestBed } from '@angular/core/testing';

import { MyFinanceService } from './my-finance.service';

describe('MyFinanceService', () => {
  let service: MyFinanceService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(MyFinanceService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
