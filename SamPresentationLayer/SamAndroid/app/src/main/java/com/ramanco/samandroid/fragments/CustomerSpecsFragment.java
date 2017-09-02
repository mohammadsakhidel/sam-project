package com.ramanco.samandroid.fragments;

import android.content.Intent;
import android.os.Bundle;
import android.support.v4.app.Fragment;
import android.text.TextUtils;
import android.view.LayoutInflater;
import android.view.Menu;
import android.view.MenuInflater;
import android.view.MenuItem;
import android.view.View;
import android.view.ViewGroup;

import com.google.gson.Gson;
import com.ramanco.samandroid.R;
import com.ramanco.samandroid.activities.MainActivity;
import com.ramanco.samandroid.api.dtos.CustomerDto;
import com.ramanco.samandroid.consts.Patterns;
import com.ramanco.samandroid.exceptions.ValidationException;
import com.ramanco.samandroid.utils.ExceptionManager;
import com.ramanco.samandroid.utils.PrefUtil;
import com.ramanco.samandroid.utils.TextUtility;
import com.rey.material.widget.EditText;

public class CustomerSpecsFragment extends Fragment {

    //region Fields:
    Runnable onOkAction;
    CustomerDto customer;
    //endregion

    //region Ctors:
    public CustomerSpecsFragment() {
    }
    //endregion

    //region Overrides:
    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {

        View fragmentView = inflater.inflate(R.layout.fragment_customer_specs, container, false);

        try {
            setHasOptionsMenu(true);
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

                View v = getView();
                if (v != null) {
                    //region gather inputs:
                    EditText etFullName = (EditText) v.findViewById(R.id.et_fullname);
                    String fullName = etFullName.getText().toString();

                    EditText etCellPhone = (EditText) v.findViewById(R.id.et_cellphone);
                    String cellPhone = etCellPhone.getText().toString();
                    //endregion

                    //region form validation:
                    if (TextUtils.isEmpty(fullName.replace(" ", ""))
                            || TextUtils.isEmpty(cellPhone))
                        throw new ValidationException(getActivity().getResources().getString(R.string.msg_fill_required_fields));

                    if (!TextUtility.isValid(Patterns.CELLPHONE, cellPhone))
                        throw new ValidationException(getActivity().getResources().getString(R.string.msg_invalid_cellphone));
                    //endregion

                    //region set customer:
                    CustomerDto c = new CustomerDto();
                    c.setFullName(fullName);
                    c.setCellPhoneNumber(cellPhone);
                    customer = c;
                    //endregion

                    //region execute on ok action runnable:
                    if (getOnOkAction() != null) {
                        getOnOkAction().run();
                    }
                    //endregion
                }
            }

            return true;
        } catch (Exception ex) {
            ExceptionManager.handle(getActivity(), ex);
            return false;
        }
    }
    //endregion

    //region Getters & Setters:

    public Runnable getOnOkAction() {
        return onOkAction;
    }

    public void setOnOkAction(Runnable onOkAction) {
        this.onOkAction = onOkAction;
    }

    public CustomerDto getCustomer() {
        return customer;
    }
    //endregion

}
