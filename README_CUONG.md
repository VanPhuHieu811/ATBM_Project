1. Tạo role cho DIEUPHOIVIEN:

dùng ui admin tạo RL_DIEUPHOIVIEN và cấp các quyền sau cho role này:

- BENHNHAN: select, insert, update full bảng
- HSBA: Select (full bảng), Insert (full bảng), Update(MABS, MAKHOA)
- HSBA_DV: Update (MAKTV), Select (full bảng)

Sau đó cấp role này cho các Nhân viên nào có chức vụ là điều phối viên rồi cho nó login dô (VD nhân viên NV001)