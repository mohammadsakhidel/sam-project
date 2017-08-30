package com.ramanco.samandroid.fragments;


import android.app.ProgressDialog;
import android.os.Bundle;
import android.support.v4.app.Fragment;
import android.support.v4.content.ContextCompat;
import android.text.TextUtils;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.EditText;
import android.widget.FrameLayout;
import android.widget.LinearLayout;
import android.widget.TextView;
import android.widget.Toast;

import com.google.gson.Gson;
import com.ramanco.samandroid.R;
import com.ramanco.samandroid.api.dtos.TemplateFieldDto;
import com.ramanco.samandroid.exceptions.ValidationException;
import com.ramanco.samandroid.utils.ExceptionManager;
import com.ramanco.samandroid.utils.VersatileUtility;

import java.util.HashMap;
import java.util.List;
import java.util.Map;

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
                        goNext(fragmentView);
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

    private void goNext(View fragmentView) throws ValidationException {

        //region get name values:
        Map<String, String> nameValues = new HashMap<>();
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

        //region show next step:
        String templateInfo = new Gson().toJson(nameValues);
        parentView.setTemplateInfo(templateInfo);
        parentView.showFinishStep();
        //endregion

    }
    //endregion

}
