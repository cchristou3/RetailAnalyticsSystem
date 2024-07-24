export class DashboardTableCustomerDistributionByCity implements IDashboardTableCustomerDistributionByCity {
  cityName!: string;
  numberOfCustomers!: number;

  constructor(data?: IDashboardTableCustomerDistributionByCity) {
    if (data) {
      for (var property in data) {
        if (data.hasOwnProperty(property))
          (<any>this)[property] = (<any>data)[property];
      }
    }
  }

  init(_data?: any) {
    if (_data) {
      this.cityName = _data["cityName"] !== undefined ? _data["cityName"] : <any>null;
      this.numberOfCustomers = _data["numberOfCustomers"] !== undefined ? _data["numberOfCustomers"] : <any>null;
    }
  }

  static fromJS(data: any): DashboardTableCustomerDistributionByCity {
    data = typeof data === 'object' ? data : {};
    let result = new DashboardTableCustomerDistributionByCity();
    result.init(data);
    return result;
  }

  toJSON(data?: any) {
    data = typeof data === 'object' ? data : {};
    data["cityName"] = this.cityName !== undefined ? this.cityName : <any>null;
    data["numberOfCustomers"] = this.numberOfCustomers !== undefined ? this.numberOfCustomers : <any>null;
    return data;
  }

  clone(): DashboardTableCustomerDistributionByCity {
    const json = this.toJSON();
    let result = new DashboardTableCustomerDistributionByCity();
    result.init(json);
    return result;
  }
}

export interface IDashboardTableCustomerDistributionByCity {
  cityName: string;
  numberOfCustomers: number;
}
