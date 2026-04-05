using System.Windows.Forms;
using System;

private void btnLoad_Click(object sender, EventArgs e)
{
    try
    {
        Presenters.UserPresenter presenter = new Presenters.UserPresenter();
        dataGridView1.DataSource = presenter.GetUserList();
    }
    catch (System.Exception ex)
    {
        System.Windows.Forms.MessageBox.Show("Lỗi kết nối: " + ex.Message);
    }
}