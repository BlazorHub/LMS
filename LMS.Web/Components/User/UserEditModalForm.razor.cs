using Microsoft.AspNetCore.Components.Forms;

namespace LMS.Web.Components.User
{
    public partial class UserEditModalForm
    {
        private Shared.User _model;

        protected override void OnInitialized()
        {
            _model = base.Options ?? new Shared.User();
            base.OnInitialized();
        }

        private void OnFinish(EditContext editContext)
        {
            _ = base.ModalRef.CloseAsync();
        }

        private void OnFinishFailed(EditContext editContext)
        {
        }
    }
}
