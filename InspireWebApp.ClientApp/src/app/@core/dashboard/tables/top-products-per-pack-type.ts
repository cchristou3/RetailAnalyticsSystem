export class DashboardTableTopProfitableProductsPerPackType implements IDashboardTableTopProfitableProductsPerPackType {
  productPackType!: string;
  rank!: number;
  productName!: string;
  value!: number;
  contribution!: number

  constructor(data?: IDashboardTableTopProfitableProductsPerPackType) {
    if (data) {
      for (var property in data) {
        if (data.hasOwnProperty(property))
          (<any>this)[property] = (<any>data)[property];
      }
    }
  }

  init(_data?: any) {
    if (_data) {
      this.productPackType = _data["productPackType"] !== undefined ? _data["productPackType"] : <any>null;
      this.rank = _data["rank"] !== undefined ? _data["rank"] : <any>null;
      this.productName = _data["productName"] !== undefined ? _data["productName"] : <any>null;
      this.value = _data["value"] !== undefined ? _data["value"] : <any>null;
      this.contribution = _data["contribution"] !== undefined ? _data["contribution"] : <any>null;
    }
  }

  static fromJS(data: any): DashboardTableTopProfitableProductsPerPackType {
    data = typeof data === 'object' ? data : {};
    let result = new DashboardTableTopProfitableProductsPerPackType();
    result.init(data);
    return result;
  }

  toJSON(data?: any) {
    data = typeof data === 'object' ? data : {};
    data["productPackType"] = this.productPackType !== undefined ? this.productPackType : <any>null;
    data["rank"] = this.rank !== undefined ? this.rank : <any>null;
    data["productName"] = this.productName !== undefined ? this.productName : <any>null;
    data["value"] = this.value !== undefined ? this.value : <any>null;
    data["contribution"] = this.contribution !== undefined ? this.contribution : <any>null;
    return data;
  }

  clone(): DashboardTableTopProfitableProductsPerPackType {
    const json = this.toJSON();
    let result = new DashboardTableTopProfitableProductsPerPackType();
    result.init(json);
    return result;
  }
}

export interface IDashboardTableTopProfitableProductsPerPackType {
  productPackType: string;
  rank: number;
  productName: string;
  value: number;
  contribution: number;
}
