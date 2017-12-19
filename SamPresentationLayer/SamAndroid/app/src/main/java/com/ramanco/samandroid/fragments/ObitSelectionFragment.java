package com.ramanco.samandroid.fragments;


import android.app.ProgressDialog;
import android.os.Bundle;
import android.support.v4.app.Fragment;
import android.text.Editable;
import android.text.TextWatcher;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.AdapterView;
import android.widget.ListView;

import com.ramanco.samandroid.R;
import com.ramanco.samandroid.adapters.PairAdapter;
import com.ramanco.samandroid.api.dtos.ObitDto;
import com.ramanco.samandroid.api.endpoints.ObitsApiEndpoint;
import com.ramanco.samandroid.enums.ObitType;
import com.ramanco.samandroid.exceptions.CallServerException;
import com.ramanco.samandroid.objects.KeyValuePair;
import com.ramanco.samandroid.utils.ApiUtil;
import com.ramanco.samandroid.utils.EnumUtil;
import com.ramanco.samandroid.utils.ExceptionManager;
import com.ramanco.samandroid.utils.UxUtil;
import com.rey.material.widget.EditText;

import java.util.ArrayList;
import java.util.List;

import retrofit2.Response;

public class ObitSelectionFragment extends Fragment {

    //region Fields:
    ProgressDialog progress;
    SendConsolationFragment parentView;
    ObitDto[] allObits;
    //endregion

    //region Ctors:
    public ObitSelectionFragment() {
    }
    //endregion

    //region Overrides:
    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {
        View fragmentView = inflater.inflate(R.layout.fragment_obit_selection, container, false);

        try {

            if (parentView != null && parentView.getSelectedMosque() != null) {
                loadObitsAsync(parentView.getSelectedMosque().getId());
            }

            //region set on item click listener:
            ListView lvItems = (ListView) fragmentView.findViewById(R.id.lv_items);
            lvItems.setOnItemClickListener(new AdapterView.OnItemClickListener() {
                @Override
                public void onItemClick(AdapterView<?> parent, View view, int position, long id) {
                    try {
                        KeyValuePair pair = (KeyValuePair) parent.getItemAtPosition(position);
                        ObitDto obit = (ObitDto) pair.getTag();
                        if (parentView != null) {
                            parentView.setSelectedObit(obit);
                            parentView.showTemplateSelectionStep();
                        }
                    } catch (Exception ex) {
                        ExceptionManager.handle(getActivity(), ex);
                    }
                }
            });
            //endregion

            //region set on text change listener:
            EditText etSearch = (EditText) fragmentView.findViewById(R.id.et_search);
            etSearch.addTextChangedListener(new TextWatcher() {
                @Override
                public void beforeTextChanged(CharSequence s, int start, int count, int after) {

                }

                @Override
                public void onTextChanged(CharSequence s, int start, int before, int count) {
                }

                @Override
                public void afterTextChanged(Editable s) {
                    try {
                        filterObits(s.toString());
                    } catch (Exception ex) {
                        ExceptionManager.handle(getActivity(), ex);
                    }
                }
            });
            //endregion

            //region nav buttons:
            parentView.setNextVisible(false);
            parentView.setPrevVisible(true);
            parentView.setOnPreviousClickListener(new Runnable() {
                @Override
                public void run() {
                    try {
                        parentView.showMosqueSelectionStep();
                    } catch (Exception ex) {
                        ExceptionManager.handle(getActivity(), ex);
                    }
                }
            });
            //endregion

        } catch (Exception ex) {
            if (progress != null)
                progress.dismiss();
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
    private void loadObitsAsync(final int mosqueId) {
        progress = UxUtil.showProgress(getActivity());
        new Thread(new Runnable() {
            @Override
            public void run() {
                try {
                    ObitsApiEndpoint endpoint = ApiUtil.createEndpoint(ObitsApiEndpoint.class);
                    Response<ObitDto[]> response = endpoint.getHenceForwardObits(mosqueId).execute();
                    if (!response.isSuccessful())
                        throw new CallServerException(getActivity());
                    allObits = response.body();
                    //region fill list view:
                    getActivity().runOnUiThread(new Runnable() {
                        @Override
                        public void run() {
                            try {
                                KeyValuePair[] pairs = toPairs(allObits);
                                View fragmentView = getView();
                                if (fragmentView != null) {
                                    fillListView(fragmentView, pairs);
                                    progress.dismiss();
                                }
                            } catch (Exception ex) {
                                progress.dismiss();
                                ExceptionManager.handle(getActivity(), ex);
                            }
                        }
                    });
                    //endregion
                } catch (Exception ex) {
                    progress.dismiss();
                    ExceptionManager.handle(getActivity(), ex);
                }
            }
        }).start();
    }

    private void searchObitsAsync(final String query) {
        new Thread(new Runnable() {
            @Override
            public void run() {
                try {
                    ObitsApiEndpoint endpoint = ApiUtil.createEndpoint(ObitsApiEndpoint.class);
                    Response<ObitDto[]> response = endpoint.search(query).execute();
                    if (!response.isSuccessful())
                        throw new CallServerException(getActivity());
                    final ObitDto[] foundObits = response.body();
                    //region fill list view:
                    getActivity().runOnUiThread(new Runnable() {
                        @Override
                        public void run() {
                            try {
                                KeyValuePair[] pairs = toPairs(foundObits);
                                View fragmentView = getView();
                                if (fragmentView != null) {
                                    fillListView(fragmentView, pairs);
                                }
                            } catch (Exception ex) {
                                ExceptionManager.handle(getActivity(), ex);
                            }
                        }
                    });
                    //endregion
                } catch (Exception ex) {
                    ExceptionManager.handle(getActivity(), ex);
                }
            }
        }).start();
    }

    private void filterObits(String text) {
        if (allObits != null) {
            //region search in loaded obits:
            // filter obits:
            List<ObitDto> filteredObits = new ArrayList<>();
            for (ObitDto obit : allObits) {
                if (obit.getTitle().contains(text) || text.contains(obit.getTitle())) {
                    filteredObits.add(obit);
                }
            }
            // load list view:
            View fragmentView = getView();
            if (fragmentView != null) {
                KeyValuePair[] pairs = toPairs(filteredObits.toArray(new ObitDto[filteredObits.size()]));
                fillListView(fragmentView, pairs);
            }
            //endregion
        } else {
            //region search from server:
            if (text.replaceAll("\\s", "").length() >= 4) {
                searchObitsAsync(text);
            }
            //endregion
        }
    }

    private KeyValuePair[] toPairs(ObitDto[] dtos) {
        KeyValuePair[] pairs = new KeyValuePair[dtos.length];
        for (int i = 0; i < dtos.length; i++) {
            ObitDto o = dtos[i];
            KeyValuePair pair = new KeyValuePair(Integer.toString(o.getId()), o.getTitle());
            pair.setDesc(String.format("%s: %s",
                    getActivity().getResources().getString(R.string.obit_type),
                    EnumUtil.getObitTypeText(getActivity(), ObitType.valueOf(o.getObitType()))));
            pair.setTag(o);
            pairs[i] = pair;
        }
        return pairs;
    }

    private void fillListView(View fragmentView, KeyValuePair[] pairs) {
        PairAdapter adapter = new PairAdapter(getActivity(), pairs);
        ListView lvItems = (ListView) fragmentView.findViewById(R.id.lv_items);
        lvItems.setAdapter(adapter);
    }
    //endregion

}