export class DashboardTableTopProfitableProductsPerTag implements IDashboardTableTopProfitableProductsPerTag {
  productTag!: string;
  rank!: number;
  productName!: string;
  value!: number;
  contribution!: number

  constructor(data?: IDashboardTableTopProfitableProductsPerTag) {
    if (data) {
      for (var property in data) {
        if (data.hasOwnProperty(property))
          (<any>this)[property] = (<any>data)[property];
      }
    }
  }

  init(_data?: any) {
    if (_data) {
      this.productTag = _data["productTag"] !== undefined ? _data["productTag"] : <any>null;
      this.rank = _data["rank"] !== undefined ? _data["rank"] : <any>null;
      this.productName = _data["productName"] !== undefined ? _data["productName"] : <any>null;
      this.value = _data["value"] !== undefined ? _data["value"] : <any>null;
      this.contribution = _data["contribution"] !== undefined ? _data["contribution"] : <any>null;
    }
  }

  static fromJS(data: any): DashboardTableTopProfitableProductsPerTag {
    data = typeof data === 'object' ? data : {};
    let result = new DashboardTableTopProfitableProductsPerTag();
    result.init(data);
    return result;
  }

  toJSON(data?: any) {
    data = typeof data === 'object' ? data : {};
    data["productTag"] = this.productTag !== undefined ? this.productTag : <any>null;
    data["rank"] = this.rank !== undefined ? this.rank : <any>null;
    data["productName"] = this.productName !== undefined ? this.productName : <any>null;
    data["value"] = this.value !== undefined ? this.value : <any>null;
    data["contribution"] = this.contribution !== undefined ? this.contribution : <any>null;
    return data;
  }

  clone(): DashboardTableTopProfitableProductsPerTag {
    const json = this.toJSON();
    let result = new DashboardTableTopProfitableProductsPerTag();
    result.init(json);
    return result;
  }
}

export interface IDashboardTableTopProfitableProductsPerTag {
  productTag: string;
  rank: number;
  productName: string;
  value: number;
  contribution: number;
}
