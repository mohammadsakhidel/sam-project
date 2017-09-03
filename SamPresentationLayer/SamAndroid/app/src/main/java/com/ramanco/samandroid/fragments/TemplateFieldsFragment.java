package com.ramanco.samandroid.fragments;


import android.app.ProgressDialog;
import android.os.Bundle;
import android.support.v4.app.Fragment;
import android.support.v4.content.ContextCompat;
import android.text.TextUtils;
import android.view.LayoutInflater;
import android.view.Menu;
import android.view.MenuInflater;
import android.view.MenuItem;
import android.view.View;
import android.view.ViewGroup;
import android.widget.EditText;
import android.widget.FrameLayout;
import android.widget.LinearLayout;
import android.widget.TextView;
import android.widget.Toast;

import com.google.gson.Gson;
import com.ramanco.samandroid.R;
import com.ramanco.samandroid.api.dtos.ConsolationDto;
import com.ramanco.samandroid.api.dtos.CustomerDto;
import com.ramanco.samandroid.api.dtos.TemplateFieldDto;
import com.ramanco.samandroid.api.endpoints.ConsolationsApiEndpoint;
import com.ramanco.samandroid.enums.ConsolationStatus;
import com.ramanco.samandroid.enums.PaymentStatus;
import com.ramanco.samandroid.exceptions.CallServerException;
import com.ramanco.samandroid.exceptions.ValidationException;
import com.ramanco.samandroid.utils.ApiUtil;
import com.ramanco.samandroid.utils.ExceptionManager;
import com.ramanco.samandroid.utils.PrefUtil;
import com.ramanco.samandroid.utils.UxUtil;
import com.ramanco.samandroid.utils.VersatileUtility;

import java.io.IOException;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

import retrofit2.Response;

public class TemplateFieldsFragment extends Fragment {

    //region Fields:
    ProgressDialog progress;
    SendConsolationFragment parentView;
    //endregion

    //region Ctors:
    public TemplateFieldsFragment() {
    }
    //endregion

    //region Overrides:
    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
        final View fragmentView = inflater.inflate(R.layout.fragment_template_fields, container, false);

        try {

            setHasOptionsMenu(true);
            loadFieldsAsync(fragmentView);

            //region nav buttons:
            parentView.setPrevVisible(true);
            parentView.setOnPreviousClickListener(new Runnable() {
                @Override
                public void run() {
                    try {
                        parentView.showTemplateSelectionStep();
                    } catch (Exception ex) {
                        ExceptionManager.handle(getActivity(), ex);
                    }
                }
            });

            parentView.setNextVisible(true);
            parentView.setOnNextClickListener(new Runnable() {
                @Override
                public void run() {
                    try {
                        go();
                    } catch (Exception ex) {
                        ExceptionManager.handle(getActivity(), ex);
                    }
                }
            });
            //endregion

        } catch (Exception ex) {
            ExceptionManager.handle(getActivity(), ex);
        }

        return fragmentView;
    }

    @Override
    public void onCreateOptionsMenu(Menu menu, MenuInflater inflater) {
        try {

            inflater.inflate(R.menu.ok_options_menu, menu);

        } catch (Exception ex) {
            ExceptionManager.handle(getActivity(), ex);
        }
    }

    @Override
    public boolean onOptionsItemSelected(MenuItem item) {
        try {
            if (item.getItemId() == R.id.mi_ok) {
                go();
            }
            return true;
        } catch (Exception ex) {
            ExceptionManager.handle(getActivity(), ex);
            return false;
        }
    }

    //endregion

    //region Getters & Setters:

    public SendConsolationFragment getParentView() {
        return parentView;
    }

    public void setParentView(SendConsolationFragment parentView) {
        this.parentView = parentView;
    }

    //endregion

    //region Methods:
    private void loadFieldsAsync(View fragmentView) {

        ViewGroup lyFields = (ViewGroup) fragmentView.findViewById(R.id.ly_fields);
        TemplateFieldDto[] fields = parentView.getSelectedTemplate().getTemplateFields();
        for (TemplateFieldDto field : fields) {
            //display name text view:
            TextView txtDisplayName = new TextView(getActivity());
            txtDisplayName.setText(String.format("%s:", field.getDisplayName()));
            LinearLayout.LayoutParams txtDisplayNameParams = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.WRAP_CONTENT, LinearLayout.LayoutParams.WRAP_CONTENT);
            txtDisplayNameParams.setMargins(0,
                    VersatileUtility.toDPs(getActivity(), (int) getActivity().getResources().getDimension(R.dimen.widget_margin)),
                    0, 0);
            txtDisplayName.setLayoutParams(txtDisplayNameParams);
            lyFields.addView(txtDisplayName);
            //endregion

            //region edit text:
            EditText etValue = new EditText(getActivity());
            etValue.setSingleLine();
            etValue.setLines(1);
            etValue.setTextColor(ContextCompat.getColor(getActivity(), R.color.colorTextPrimary));
            etValue.setHintTextColor(ContextCompat.getColor(getActivity(), R.color.colorTextSecondary));
            etValue.setBackground(ContextCompat.getDrawable(getActivity(), R.drawable.bg_edit_text_2));
            int etPadding = (int) getActivity().getResources().getDimension(R.dimen.widget_margin);
            etValue.setPadding(etPadding, etPadding, etPadding, etPadding);
            LinearLayout.LayoutParams etValueParams = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MATCH_PARENT, LinearLayout.LayoutParams.WRAP_CONTENT);
            etValueParams.setMargins(0,
                    VersatileUtility.toDPs(getActivity(), (int) getActivity().getResources().getDimension(R.dimen.widget_margin)),
                    0, VersatileUtility.toDPs(getActivity(), (int) getActivity().getResources().getDimension(R.dimen.widget_margin)));
            etValue.setLayoutParams(etValueParams);
            etValue.setTag(field.getName());
            lyFields.addView(etValue);
            //endregion

            //region description:
            TextView txtDesc = new TextView(getActivity());
            txtDesc.setText(field.getDescription());
            txtDesc.setTextColor(ContextCompat.getColor(getActivity(), R.color.colorTextSecondary));
            txtDesc.setTextSize(VersatileUtility.toDPs(getActivity(), (int) getActivity().getResources().getDimension(R.dimen.font_size_tiny)));
            lyFields.addView(txtDesc);
            //endregion
        }
    }

    private void go() throws ValidationException, IOException, CallServerException {

        View fragmentView = getView();
        if (fragmentView != null) {

            //region get name values:
            final Map<String, String> nameValues = new HashMap<>();
            ViewGroup container = (ViewGroup) fragmentView.findViewById(R.id.ly_fields);
            for (int i = 0; i < container.getChildCount(); i++) {
                View child = container.getChildAt(i);
                if (child instanceof EditText) {
                    EditText etField = (EditText) child;
                    nameValues.put(etField.getTag().toString(), etField.getText().toString());
                }
            }
            //endregion

            //region validation:
            boolean isValid = true;
            for (Map.Entry<String, String> entry : nameValues.entrySet()) {
                if (TextUtils.isEmpty(entry.getKey().replace(" ", "")) ||
                        TextUtils.isEmpty(entry.getValue().replace(" ", ""))) {
                    isValid = false;
                }
            }

            if (!isValid)
                throw new ValidationException(getActivity().getResources().getString(R.string.msg_fill_required_fields));
            //endregion

            //region call server to save consolation and show preview step:
            final ProgressDialog progress = UxUtil.showProgress(getActivity());

            new Thread(new Runnable() {
                @Override
                public void run() {
                    try {

                        //region create consolation object:
                        final String templateInfo = new Gson().toJson(nameValues);
                        CustomerDto customer = new Gson().fromJson(PrefUtil.getCustomerInfo(getActivity()), CustomerDto.class);

                        ConsolationDto dto = new ConsolationDto();
                        dto.setObitID(parentView.getSelectedObit().getId());
                        dto.setCustomer(customer);
                        dto.setTemplateID(parentView.getSelectedTemplate().getId());
                        dto.setTemplateInfo(templateInfo);
                        dto.setAudience(nameValues.get("Audience"));
                        dto.setFrom(nameValues.get("From"));
                        dto.setPaymentStatus(PaymentStatus.pending.toString());
                        dto.setStatus(ConsolationStatus.pending.toString());
                        //endregion
                        //region call api:
                        ConsolationsApiEndpoint endpoint = ApiUtil.createEndpoint(ConsolationsApiEndpoint.class);
                        Response<Integer> response = endpoint.create(dto).execute();
                        if (!response.isSuccessful())
                            throw new CallServerException(getActivity());

                        final int consolationId = response.body();
                        //endregion
                        //region show next:
                        getActivity().runOnUiThread(new Runnable() {
                            @Override
                            public void run() {
                                try {
                                    progress.dismiss();

                                    parentView.setTemplateInfo(templateInfo);
                                    parentView.setCreatedConsolationId(consolationId);
                                    parentView.showPreviewStep();
                                } catch (Exception ex) {
                                    ExceptionManager.handle(getActivity(), ex);
                                }
                            }
                        });
                        //endregion

                    } catch (Exception ex) {
                        //region hide progress:
                        getActivity().runOnUiThread(new Runnable() {
                            @Override
                            public void run() {
                                progress.dismiss();
                            }
                        });
                        //endregion
                        ExceptionManager.handle(getActivity(), ex);
                    }
                }
            }).start();
            //endregion

        }
    }
    //endregion

}
